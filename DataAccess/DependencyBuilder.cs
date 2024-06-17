using Microsoft.Extensions.DependencyInjection;

namespace DataAccess
{
    public class DependencyBuilder
    {
        public static void InitializeInMemoryDependencies(IServiceCollection services)
        {
            services.AddSingleton<IPostsRepository, MemoryPostsRepository>();
            services.AddSingleton<IUsersRepository, MemoryUsersRepository>();
        }
    }
}
