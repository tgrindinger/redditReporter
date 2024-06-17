using Contracts;
using DataAccess;

namespace DataAccessTests.Builders
{
    internal class MemoryPostsRepositoryBuilder
    {
        private MemoryPostsRepository _repo = new();
        private Random _random = new();

        public MemoryPostsRepository Build()
        {
            return _repo;
        }

        public MemoryPostsRepositoryBuilder WithPosts(int numPosts)
        {
            foreach (var _ in Enumerable.Range(0, numPosts))
            {
                PostSummary summary = new(RandomString(), RandomString(), RandomInt());
                _repo.UpdatePostSummary(summary);
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
