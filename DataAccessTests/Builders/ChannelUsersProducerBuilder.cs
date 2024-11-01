using Contracts;
using DataAccess;
using sysChannels = System.Threading.Channels;

namespace DataAccessTests.Builders
{
    internal class ChannelUsersProducerBuilder
    {
        private readonly sysChannels.Channel<UserSummary> _channel;
        private readonly ChannelUsersProducer _channelUsersProducer;

        public sysChannels.Channel<UserSummary> Channel => _channel;

        public ChannelUsersProducerBuilder()
        {
            _channel = sysChannels.Channel.CreateUnbounded<UserSummary>();
            _channelUsersProducer = new ChannelUsersProducer(_channel);
        }

        public async Task<ChannelUsersProducerBuilder> WithUsers(int numSummaries)
        {
            await Task.WhenAll(Enumerable.Range(0, numSummaries)
                .Select(_ => DataGenerators.RandomUserSummary())
                .Select(summary => _channelUsersProducer.UpdateUserSummary(summary))
                .ToArray());
            return this;
        }

        public ChannelUsersProducer Build()
        {
            return _channelUsersProducer;
        }
    }
}
