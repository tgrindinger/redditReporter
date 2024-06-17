using Contracts;
using DataAccess;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

namespace Services
{
    internal class RedditNetRedditMonitor : BackgroundService, IRedditMonitor
    {
        private readonly ILogger<RedditNetRedditMonitor> _logger;
        private readonly IPostsRepository _postsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly RedditAuth _redditAuth;
        private readonly RedditSettings _redditSettings;

        public RedditNetRedditMonitor(
            ILogger<RedditNetRedditMonitor> logger,
            IPostsRepository postsRepository,
            IUsersRepository usersRepository,
            RedditAuth redditAuth,
            RedditSettings redditSettings)
        {
            _logger = logger;
            _postsRepository = postsRepository;
            _usersRepository = usersRepository;
            _redditAuth = redditAuth;
            _redditSettings = redditSettings;
        }

        public void Start()
        {
            Task.Run(() => ExecuteAsync(new CancellationToken()));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Log(LogLevel.Information, "Starting monitor...");
            try
            {
                var sub = InitializeSub();
                OptionallySeedExistingPosts(sub);
                sub.Posts.NewUpdated += C_NewPostsUpdated;
                sub.Posts.TopUpdated += C_TopPostsUpdated;
                _logger.Log(LogLevel.Information, "Monitoring {sub}", sub.Name);
                sub.Posts.MonitorTop(breakOnFailure: true);
                sub.Posts.MonitorNew(breakOnFailure: true);
            }
            catch (Exception)
            {
                _logger.Log(LogLevel.Error, "Unable to start monitor. Please check your credentials in appsettings.json.");
                throw;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void OptionallySeedExistingPosts(Subreddit sub)
        {
            if (_redditSettings.SeedTopPosts)
            {
                var topPosts = sub.Posts.GetTop();
                RecordPostChanges(topPosts);
            }
            if (_redditSettings.SeedNewPosts)
            {
                var newPosts = sub.Posts.GetNew();
                RecordUserChanges(newPosts);
            }
        }

        private Subreddit InitializeSub()
        {
            var reddit = new RedditClient(
                appId: _redditAuth.AppId,
                refreshToken: _redditAuth.RefreshToken,
                accessToken: _redditAuth.AccessToken);
            _logger.Log(LogLevel.Information, "Client initialized: {reddit}", reddit);
            var sub = reddit.Subreddit(_redditSettings.TrackedSub);
            return sub;
        }

        private void C_TopPostsUpdated(object? sender, PostsUpdateEventArgs e)
        {
            _logger.Log(LogLevel.Information, "Top post updated: {added}", e.Added.Count);
            RecordPostChanges(e.Added);
        }

        private void C_NewPostsUpdated(object? sender, PostsUpdateEventArgs e)
        {
            _logger.Log(LogLevel.Information, "Received new posts: {added}", e.Added.Count);
            RecordUserChanges(e.Added);
        }

        private void RecordPostChanges(List<Post> topPosts)
        {
            var summaries = topPosts.Select(post => new PostSummary(post.Id, post.Title, post.UpVotes));
            foreach (var summary in summaries)
            {
                _postsRepository.UpdatePostSummary(summary);
                _logger.Log(LogLevel.Information, "Updated Post {title}: {upvotes}", summary.Title, summary.Upvotes);
            }
        }

        private void RecordUserChanges(List<Post> newPosts)
        {
            var summaries = newPosts.Select(post => new UserSummary(post.Author, 1));
            foreach (var summary in summaries)
            {
                _usersRepository.UpdateUserSummary(summary);
                _logger.Log(LogLevel.Information, "Found new post by {author}", summary.Name);
            }
        }
    }
}
