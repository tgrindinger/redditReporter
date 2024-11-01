using Contracts;
using DataAccess;
using RedditReporter.Controllers;
using System.Threading.Channels;

namespace RedditReporterTests.Builders
{
    internal class UsersControllerBuilder
    {
        private readonly ChannelUsersProducer _channelUsersProducer;
        private readonly ChannelUsersConsumer _channelUsersConsumer;
        private readonly UsersController _controller;

        public UsersControllerBuilder()
        {
            var channel = Channel.CreateUnbounded<UserSummary>();
            _channelUsersProducer = new ChannelUsersProducer(channel);
            _channelUsersConsumer = new ChannelUsersConsumer(channel);
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                _channelUsersConsumer.Start(cancellationTokenSource.Token);
            }
            _controller = new UsersController(_channelUsersConsumer);
        }

        public async Task<UsersControllerBuilder> WithUsers(int numUsers)
        {
            var tasks = Enumerable.Range(0, numUsers)
                .Select(_ => DataGenerators.RandomUserSummary())
                .Select(_channelUsersProducer.UpdateUserSummary);
            await Task.WhenAll(tasks);
            return this;
        }

        public async Task<UsersControllerBuilder> WithUser(UserSummary postSummary)
        {
            await _channelUsersProducer.UpdateUserSummary(postSummary);
            return this;
        }

        public async Task<UsersControllerBuilder> IsFlushed(int numItems = 1, int millisToWait = 100)
        {
            var startTime = DateTime.UtcNow;
            while (_channelUsersConsumer.GetUserSummaries().Count() < numItems
                && DateTime.UtcNow - startTime < TimeSpan.FromMilliseconds(millisToWait))
            {
                await Task.Delay(100);
            }
            return this;
        }

        public UsersController Build()
        {
            return _controller;
        }
    }
}
