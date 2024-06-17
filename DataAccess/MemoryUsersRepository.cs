using Contracts;
using System.Collections.Concurrent;

namespace DataAccess
{
    internal class MemoryUsersRepository : IUsersRepository
    {
        private readonly ConcurrentDictionary<string, UserSummary> _userSummaries = new();

        public IEnumerable<UserSummary> GetUserSummaries()
        {
            return _userSummaries.Values.OrderByDescending(summary => summary.Posts);
        }

        public void UpdateUserSummary(UserSummary userSummary)
        {
            _userSummaries.AddOrUpdate(userSummary.Name, userSummary,
                (id, us) => new UserSummary(us.Name, us.Posts + userSummary.Posts));
        }
    }
}
