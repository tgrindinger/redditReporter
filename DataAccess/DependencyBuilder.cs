using Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace DataAccess
{
    public static class DependencyBuilder
    {
        public static void InitializeDependencies(IServiceCollection services)
        {
            services.AddSingleton(Channel.CreateUnbounded<PostSummary>(
                new UnboundedChannelOptions
                {
                    SingleReader = false,
                    SingleWriter = false
                }));
            services.AddSingleton(Channel.CreateUnbounded<UserSummary>(
                new UnboundedChannelOptions
                {
                    SingleReader = false,
                    SingleWriter = false
                }));
            services.AddSingleton<IPostsConsumer, ChannelPostsConsumer>();
            services.AddSingleton<IPostsProducer, ChannelPostsProducer>();
            services.AddSingleton<IUsersConsumer, ChannelUsersConsumer>();
            services.AddSingleton<IUsersProducer, ChannelUsersProducer>();
        }
    }
}
