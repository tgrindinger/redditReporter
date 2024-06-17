using Services;

namespace RedditReporter
{
    public class Program
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

            StartRedditMonitor(app.Services);

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
            DataAccess.DependencyBuilder.InitializeInMemoryDependencies(services);
            Services.DependencyBuilder.InitializeRedditNetDependencies(services, configuration);
        }

        private static void StartRedditMonitor(IServiceProvider services)
        {
            var monitor = services.GetService<IRedditMonitor>();
            if (monitor == null)
            {
                throw new ArgumentNullException(nameof(monitor), "service not initialized");
            }
            monitor.Start();
        }
    }
}