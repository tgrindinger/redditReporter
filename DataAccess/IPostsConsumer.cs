using Contracts;

namespace DataAccess
{
    public interface IPostsConsumer
    {
        void Start(CancellationToken cancellationToken);
        IEnumerable<PostSummary> GetPostSummaries();
    }
}
