using Contracts;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace RedditReporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostsRepository _postsRepository;

        public PostsController(
            ILogger<PostsController> logger,
            IPostsRepository postsRepository)
        {
            _logger = logger;
            _postsRepository = postsRepository;
        }

        [HttpGet(Name = "GetTopPosts")]
        public IEnumerable<PostSummary> GetTopPosts()
        {
            var postSummaries = _postsRepository.GetPostSummaries();
            return postSummaries.ToArray();
        }
    }
}