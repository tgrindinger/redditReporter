using Contracts;
using DataAccess;
using sysChannels = System.Threading.Channels;

namespace DataAccessTests.Builders
{
    internal class ChannelPostsProducerBuilder
    {
        private readonly sysChannels.Channel<PostSummary> _channel;
        private readonly ChannelPostsProducer _channelPostsProducer;

        public sysChannels.Channel<PostSummary> Channel => _channel;

        public ChannelPostsProducerBuilder()
        {
            _channel = sysChannels.Channel.CreateUnbounded<PostSummary>();
            _channelPostsProducer = new ChannelPostsProducer(_channel);
        }

        public async Task<ChannelPostsProducerBuilder> WithPosts(int numSummaries)
        {
            await Task.WhenAll(Enumerable.Range(0, numSummaries)
                .Select(_ => DataGenerators.RandomPostSummary())
                .Select(summary => _channelPostsProducer.UpdatePostSummary(summary))
                .ToArray());
            return this;
        }

        public ChannelPostsProducer Build()
        {
            return _channelPostsProducer;
        }
    }
}
