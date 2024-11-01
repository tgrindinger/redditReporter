using Contracts;
using DataAccess;
using Microsoft.Extensions.Logging;
using Reddit.Controllers;

namespace Services
{
    internal class PostsProcessor(ILogger<PostsProcessor> logger, IPostsProducer postsProducer, IUsersProducer usersProducer)
    {
        private readonly ILogger<PostsProcessor> _logger = logger;
        private readonly IPostsProducer _postsProducer = postsProducer;
        private readonly IUsersProducer _usersProducer = usersProducer;

        public void RecordPostChanges(IEnumerable<Post> topPosts)
        {
            var summaries = topPosts.Select(post => new PostSummary(post.Id, post.Title, post.UpVotes));
            var tasks = summaries.Select(s =>
            {
                _logger.Log(LogLevel.Information, "Updated Post {Title}: {Upvotes}", s.Title, s.Upvotes);
                return _postsProducer.UpdatePostSummary(s);
            });
            Task.WaitAll(tasks.ToArray());
        }

        public void RecordUserChanges(IEnumerable<Post> newPosts)
        {
            var summaries = newPosts.Select(post => new UserSummary(post.Author, 1));
            var tasks = summaries.Select(s =>
            {
                _logger.Log(LogLevel.Information, "Found new post by {Author}", s.Name);
                return _usersProducer.UpdateUserSummary(s);
            });
            Task.WaitAll(tasks.ToArray());
        }
    }
}
