using Contracts;
using DataAccess;
using RedditReporter.Controllers;
using System.Threading.Channels;

namespace RedditReporterTests.Builders
{
    internal class PostsControllerBuilder
    {
        private readonly ChannelPostsProducer _channelPostsProducer;
        private readonly ChannelPostsConsumer _channelPostsConsumer;
        private readonly PostsController _controller;

        public PostsControllerBuilder()
        {
            var channel = Channel.CreateUnbounded<PostSummary>();
            _channelPostsProducer = new ChannelPostsProducer(channel);
            _channelPostsConsumer = new ChannelPostsConsumer(channel);
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                _channelPostsConsumer.Start(cancellationTokenSource.Token);
            }
            _controller = new PostsController(_channelPostsConsumer);
        }

        public async Task<PostsControllerBuilder> WithPosts(int numPosts)
        {
            var tasks = Enumerable.Range(0, numPosts)
                .Select(_ => DataGenerators.RandomPostSummary())
                .Select(_channelPostsProducer.UpdatePostSummary);
            await Task.WhenAll(tasks);
            return this;
        }

        public async Task<PostsControllerBuilder> WithPost(PostSummary postSummary)
        {
            await _channelPostsProducer.UpdatePostSummary(postSummary);
            return this;
        }

        public async Task<PostsControllerBuilder> IsFlushed(int numItems = 1, int millisToWait = 100)
        {
            var startTime = DateTime.UtcNow;
            while (_channelPostsConsumer.GetPostSummaries().Count() < numItems
                && DateTime.UtcNow - startTime < TimeSpan.FromMilliseconds(millisToWait))
            {
                await Task.Delay(100);
            }
            return this;
        }

        public PostsController Build()
        {
            return _controller;
        }
    }
}
