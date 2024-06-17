using Contracts;

namespace DataAccess
{
    public interface IUsersRepository
    {
        IEnumerable<UserSummary> GetUserSummaries();
        void UpdateUserSummary(UserSummary userSummary);
    }
}
