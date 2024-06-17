using Contracts;

namespace DataAccess
{
    public interface IPostsRepository
    {
        IEnumerable<PostSummary> GetPostSummaries();
        void UpdatePostSummary(PostSummary postSummary);
    }
}
