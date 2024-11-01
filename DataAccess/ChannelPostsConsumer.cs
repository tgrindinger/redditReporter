using Contracts;
using System.Collections.Immutable;
using System.Threading.Channels;

namespace DataAccess
{
    internal class ChannelPostsConsumer(Channel<PostSummary> channel) : IPostsConsumer
    {
        private readonly Channel<PostSummary> _channel = channel;
        private readonly Dictionary<string, PostSummary> _posts = [];
        private IEnumerable<PostSummary> _readPosts = [];

        public void Start(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var summary = await _channel.Reader.ReadAsync(cancellationToken);
                    _posts[summary.Id] = summary;
                    if (_channel.Reader.Count == 0)
                    {
                        _readPosts = _posts.Values.OrderByDescending(post => post.Upvotes);
                    }
                }
            }, cancellationToken);
        }

        public IEnumerable<PostSummary> GetPostSummaries()
        {
            return _readPosts;
        }
    }
}
