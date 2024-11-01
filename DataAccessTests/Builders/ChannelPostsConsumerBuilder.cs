using Contracts;
using DataAccess;
using System.Threading.Channels;

namespace DataAccessTests.Builders
{
    internal class ChannelPostsConsumerBuilder : IDisposable
    {
        private readonly Channel<PostSummary> _channel;
        private readonly ChannelPostsConsumer _channelPostsConsumer;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool disposedValue = false;

        public ChannelPostsConsumerBuilder()
        {
            _channel = Channel.CreateUnbounded<PostSummary>();
            _channelPostsConsumer = new ChannelPostsConsumer(_channel);
            _cancellationTokenSource = new CancellationTokenSource();
            _channelPostsConsumer.Start(_cancellationTokenSource.Token);
        }

        public ChannelPostsConsumerBuilder IsCancelled()
        {
            _cancellationTokenSource.Cancel();
            return this;
        }

        public async Task<ChannelPostsConsumerBuilder> WithPost(PostSummary postSummary)
        {
            await _channel.Writer.WriteAsync(postSummary);
            return this;
        }

        public async Task<ChannelPostsConsumerBuilder> WithPosts(int numSummaries)
        {
            await Task.WhenAll(Enumerable.Range(0, numSummaries)
                .Select(_ => DataGenerators.RandomPostSummary())
                .Select(summary => _channel.Writer.WriteAsync(summary).AsTask())
                .ToArray());
            return this;
        }

        public async Task<ChannelPostsConsumer> Build()
        {
            await Task.Delay(100);
            return _channelPostsConsumer;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
