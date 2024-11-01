using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit;
using Reddit.Controllers;
using Reddit.Controllers.EventArgs;

namespace Services
{
    internal class RedditNetRedditMonitor(
        ILogger<RedditNetRedditMonitor> logger,
        PostsProcessor postsProcessor,
        RedditAuth redditAuth,
        RedditSettings redditSettings) : BackgroundService, IRedditMonitor
    {
        private readonly ILogger<RedditNetRedditMonitor> _logger = logger;
        private readonly PostsProcessor _postsProcessor = postsProcessor;
        private readonly RedditAuth _redditAuth = redditAuth;
        private readonly RedditSettings _redditSettings = redditSettings;

        public void Start(CancellationToken stoppingToken)
        {
            Task.Run(() => ExecuteAsync(stoppingToken), stoppingToken);
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
                _logger.Log(LogLevel.Information, "Monitoring {Sub}", sub.Name);
                sub.Posts.MonitorTop(breakOnFailure: true);
                sub.Posts.MonitorNew(breakOnFailure: true);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Unable to start monitor. Please check your credentials in appsettings.json.");
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
                _postsProcessor.RecordPostChanges(topPosts);
            }
            if (_redditSettings.SeedNewPosts)
            {
                var newPosts = sub.Posts.GetNew();
                _postsProcessor.RecordUserChanges(newPosts);
            }
        }

        private Subreddit InitializeSub()
        {
            var reddit = new RedditClient(
                appId: _redditAuth.AppId,
                refreshToken: _redditAuth.RefreshToken,
                accessToken: _redditAuth.AccessToken);
            _logger.Log(LogLevel.Information, "Client initialized: {Reddit}", reddit);
            var sub = reddit.Subreddit(_redditSettings.TrackedSub);
            return sub;
        }

        private void C_TopPostsUpdated(object? sender, PostsUpdateEventArgs e)
        {
            _postsProcessor.RecordPostChanges(e.Added);
        }

        private void C_NewPostsUpdated(object? sender, PostsUpdateEventArgs e)
        {
            _postsProcessor.RecordUserChanges(e.Added);
        }
    }
}
