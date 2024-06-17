using Contracts;
using DataAccess;

namespace DataAccessTests.Builders
{
    internal class MemoryUsersRepositoryBuilder
    {
        private MemoryUsersRepository _repo = new();
        private Random _random = new();

        public MemoryUsersRepository Build()
        {
            return _repo;
        }

        public MemoryUsersRepositoryBuilder WithUsers(int numUsers)
        {
            foreach (var _ in Enumerable.Range(0, numUsers))
            {
                UserSummary summary = new(RandomString(), RandomInt());
                _repo.UpdateUserSummary(summary);
            }
            return this;
        }

        private string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        private int RandomInt()
        {
            return _random.Next(1000);
        }
    }
}
