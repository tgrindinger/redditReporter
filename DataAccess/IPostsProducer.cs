using Contracts;

namespace DataAccess
{
    public interface IPostsProducer
    {
        Task UpdatePostSummary(PostSummary postSummary);
    }
}
