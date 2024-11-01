using Contracts;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace RedditReporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController(IPostsConsumer postsConsumer) : ControllerBase
    {
        private readonly IPostsConsumer _postsConsumer = postsConsumer;

        [HttpGet(Name = "GetTopPosts")]
        public IEnumerable<PostSummary> GetTopPosts()
        {
            var postSummaries = _postsConsumer.GetPostSummaries();
            return postSummaries.ToArray();
        }
    }
}