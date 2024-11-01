using Contracts;

namespace DataAccess
{
    public interface IUsersConsumer
    {
        void Start(CancellationToken cancellationToken);
        IEnumerable<UserSummary> GetUserSummaries();
    }
}
