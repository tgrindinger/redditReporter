using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public class DependencyBuilder
    {
        public static void InitializeRedditNetDependencies(IServiceCollection services, IConfiguration configuration)
        {
            InitializeRedditAuth(services, configuration);
            InitializeRedditSettings(services, configuration);
            services.AddSingleton<IRedditMonitor, RedditNetRedditMonitor>();
        }

        private static void InitializeRedditSettings(IServiceCollection services, IConfiguration configuration)
        {
            var settingsSection = configuration.GetSection("RedditSettings");
            var sub = RetrieveSetting("TrackedSub", settingsSection);
            var seedTopPosts = RetrieveSetting("SeedTopPosts", settingsSection).ToLower() == "true";
            var seedNewPosts = RetrieveSetting("SeedNewPosts", settingsSection).ToLower() == "true";
            services.AddSingleton(new RedditSettings(sub, seedTopPosts, seedNewPosts));
        }

        private static void InitializeRedditAuth(IServiceCollection services, IConfiguration configuration)
        {
            var authSection = configuration.GetSection("RedditAuth");
            var appId = RetrieveSetting("appId", authSection);
            var refreshToken = RetrieveSetting("refreshToken", authSection);
            var accessKey = RetrieveSetting("accessToken", authSection);
            services.AddSingleton(new RedditAuth(appId, refreshToken, accessKey));
        }

        private static string RetrieveSetting(string key, IConfigurationSection section)
        {
            var value = section[key];
            if (value == null || value.Length == 0)
            {
                throw new ArgumentNullException(key, $"must be provided in the {section.Path} section of the appsettings.json file");
            }
            return value;
        }
    }
}
