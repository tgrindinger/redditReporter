using Contracts;
using System.Collections.Immutable;
using System.Threading.Channels;

namespace DataAccess
{
    internal class ChannelUsersConsumer(Channel<UserSummary> channel) : IUsersConsumer
    {
        private readonly Channel<UserSummary> _channel = channel;
        private readonly Dictionary<string, UserSummary> _users = [];
        private IEnumerable<UserSummary> _readUsers = [];

        public void Start(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var summary = await _channel.Reader.ReadAsync(cancellationToken);
                    _users.TryGetValue(summary.Name, out var oldSummary);
                    var oldPosts = oldSummary == null ? 0 : oldSummary.Posts;
                    _users[summary.Name] = new UserSummary(summary.Name, summary.Posts + oldPosts);
                    if (_channel.Reader.Count == 0)
                    {
                        _readUsers = _users.Values.OrderByDescending(user => user.Posts);
                    }
                }
            }, cancellationToken);
        }

        public IEnumerable<UserSummary> GetUserSummaries()
        {
            return _readUsers;
        }
    }
}
