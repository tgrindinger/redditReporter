using Contracts;
using System.Collections.Concurrent;

namespace DataAccess
{
    internal class MemoryPostsRepository : IPostsRepository
    {
        private readonly ConcurrentDictionary<string, PostSummary> _postSummaries = new();

        public IEnumerable<PostSummary> GetPostSummaries()
        {
            return _postSummaries.Values.OrderByDescending(summary => summary.Upvotes);
        }

        public void UpdatePostSummary(PostSummary postSummary)
        {
            _postSummaries.AddOrUpdate(postSummary.Id, postSummary, (_, _) => postSummary);
        }
    }
}
