using DataAccess;
using Services;

namespace RedditReporter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            AddInternalDependencies(builder.Services, builder.Configuration);

            var app = builder.Build();

            using (var redditTokenSource = new CancellationTokenSource())
            using (var usersTokenSource = new CancellationTokenSource())
            using (var postsTokenSource = new CancellationTokenSource())
            {
                StartRedditMonitor(app.Services, redditTokenSource.Token);
                StartConsumers(app.Services, usersTokenSource.Token, postsTokenSource.Token);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void AddInternalDependencies(IServiceCollection services, IConfiguration configuration)
        {
            DataAccess.DependencyBuilder.InitializeDependencies(services);
            Services.DependencyBuilder.InitializeDependencies(services, configuration);
        }

        private static void StartRedditMonitor(IServiceProvider services, CancellationToken stoppingToken)
        {
            var monitor = services.GetService<IRedditMonitor>();
            if (monitor == null)
            {
                throw new TypeInitializationException(nameof(monitor), null);
            }
            monitor.Start(stoppingToken);
        }

        private static void StartConsumers(IServiceProvider services, CancellationToken usersCancellationToken, CancellationToken postsCancellationToken)
        {
            var usersConsumer = services.GetService<IUsersConsumer>();
            if (usersConsumer == null)
            {
                throw new TypeInitializationException(nameof(usersConsumer), null);
            }
            usersConsumer.Start(usersCancellationToken);

            var postsConsumer = services.GetService<IPostsConsumer>();
            if (postsConsumer == null)
            {
                throw new TypeInitializationException(nameof(postsConsumer), null);
            }
            postsConsumer.Start(postsCancellationToken);
        }
    }
}
