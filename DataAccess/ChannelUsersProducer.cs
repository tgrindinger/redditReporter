using Contracts;
using System.Threading.Channels;

namespace DataAccess
{
    internal class ChannelUsersProducer(Channel<UserSummary> channel) : IUsersProducer
    {
        private readonly Channel<UserSummary> _channel = channel;

        public async Task UpdateUserSummary(UserSummary userSummary)
        {
            await _channel.Writer.WriteAsync(userSummary);
        }
    }
}
