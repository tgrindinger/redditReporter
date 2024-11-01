using Contracts;

namespace DataAccess
{
    public interface IUsersProducer
    {
        Task UpdateUserSummary(UserSummary userSummary);
    }
}
