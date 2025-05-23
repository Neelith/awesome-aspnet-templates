using System.Reflection;
using YourProjectName.Application;
using YourProjectName.Infrastructure;
using YourProjectName.Infrastructure.Caching;
using YourProjectName.Infrastructure.Persistence;
using YourProjectName.WebApi.Infrastructure.Middlewares;

namespace YourProjectName.WebApi.Infrastructure.Setup;

internal static class DependencyInjection
{
    public static IServiceCollection AddAppServices(this WebApplicationBuilder webApplicationBuilder)
    {
        //Add logging
        ILogger startupLogger = webApplicationBuilder.AddLogging();

        //Get the services collection
        IServiceCollection services = webApplicationBuilder.Services;

        //Get configuration
        IConfiguration configuration = webApplicationBuilder.Configuration;

        //Get the database connection string
        string? dbConnectionString = configuration.GetConnectionString("YourProjectNameDb");

        //Add the redis settings to the container and get an istance of it
        RedisSettings? redisSettings = services.AddSettings<RedisSettings>(configuration, startupLogger);

        //Register services here
        services
            .AddHttpContextAccessor()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .ConfigureProblemDetails()
            .AddApplicationServices()
            .AddInfrastructureServices(startupLogger, dbConnectionString, redisSettings)
            .AddEndpoints(Assembly.GetExecutingAssembly())
            .AddOpenApiServices();

        return services;
    }

    // Configure the HTTP request pipeline.
    public static void UseAppServices(this WebApplication app)
    {
        //Enable logging
        app.UseLogging();

        //Enable global exception handling
        app.UseExceptionHandler();

        //Register all the endpoints that implement the IEndpoints interface
        app.MapEndpoints();

        //Enable OpenApi documentation and UI
        app.UseOpenApi();

        app.UseHttpsRedirection();

        //Apply database migrations
        using IServiceScope scope = app.Services.CreateScope();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        AddDatabaseMigrationsExtension.ApplyDatabaseMigrations(scope, logger);
    }
}
