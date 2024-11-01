using Contracts;
using DataAccess;
using System.Threading.Channels;

namespace DataAccessTests.Builders
{
    internal class ChannelUsersConsumerBuilder : IDisposable
    {
        private readonly Channel<UserSummary> _channel;
        private readonly ChannelUsersConsumer _channelUsersConsumer;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool disposedValue = false;

        public ChannelUsersConsumerBuilder()
        {
            _channel = Channel.CreateUnbounded<UserSummary>();
            _channelUsersConsumer = new ChannelUsersConsumer(_channel);
            _cancellationTokenSource = new CancellationTokenSource();
            _channelUsersConsumer.Start(_cancellationTokenSource.Token);
        }

        public ChannelUsersConsumerBuilder IsCancelled()
        {
            _cancellationTokenSource.Cancel();
            return this;
        }

        public async Task<ChannelUsersConsumerBuilder> WithUser(UserSummary postSummary)
        {
            await _channel.Writer.WriteAsync(postSummary);
            return this;
        }

        public async Task<ChannelUsersConsumerBuilder> WithUsers(int numSummaries)
        {
            await Task.WhenAll(Enumerable.Range(0, numSummaries)
                .Select(_ => DataGenerators.RandomUserSummary())
                .Select(summary => _channel.Writer.WriteAsync(summary).AsTask())
                .ToArray());
            return this;
        }

        public async Task<ChannelUsersConsumer> Build()
        {
            await Task.Delay(100);
            return _channelUsersConsumer;
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
