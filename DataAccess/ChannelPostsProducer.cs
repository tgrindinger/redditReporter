using Contracts;
using System.Threading.Channels;

namespace DataAccess
{
    internal class ChannelPostsProducer(Channel<PostSummary> channel) : IPostsProducer
    {
        private readonly Channel<PostSummary> _channel = channel;

        public async Task UpdatePostSummary(PostSummary postSummary)
        {
            await _channel.Writer.WriteAsync(postSummary);
        }
    }
}
