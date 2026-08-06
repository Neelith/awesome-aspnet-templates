using YourProjectName.Core;
using YourProjectName.Infrastructure;
using YourProjectName.Infrastructure.Caching;
using YourProjectName.Infrastructure.Persistence;
using YourProjectName.WebApi.DependencyInjectionExtensions;
using YourProjectName.WebApi.Middlewares;
using YourProjectName.WebApi.Settings;

namespace YourProjectName.WebApi;

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
        string dbConnectionString = configuration.GetConnectionString("YourProjectNameDb")
            ?? throw new InvalidOperationException("Connection string 'YourProjectNameDb' not found.");

        //Add the redis settings to the container and get an instance of it (optional, falls back to L1-only cache)
        RedisSettings? redisSettings = services.AddSettings<RedisSettings>(configuration, startupLogger);

        //Add the jwt settings to the container and get an instance of it
        JwtSettings jwtSettings = services.AddSettings<JwtSettings>(configuration, startupLogger)
            ?? throw new InvalidOperationException("Configuration section 'JwtSettings' not found.");

        //Load telemetry settings (optional, defaults baked into class)
        OpenTelemetrySettings telemetrySettings = services.AddSettings<OpenTelemetrySettings>(configuration, startupLogger)
            ?? new OpenTelemetrySettings();

        //Register services here
        services
            .AddTelemetry(telemetrySettings, webApplicationBuilder.Environment)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddHttpContextAccessor()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetailsServices()
            .AddAuthenticationServices(jwtSettings, webApplicationBuilder.Environment)
            .AddAuthorizationServices()
            .AddCoreServices()
            .AddInfrastructureServices(startupLogger, dbConnectionString, redisSettings)
            .AddHealthCheckServices()
            .AddEndpoints()
            .AddOpenApiServices(jwtSettings);

        return services;
    }

    // Configure the HTTP request pipeline.
    public static void UseAppServices(this WebApplication app)
    {
        //Enable logging
        app.UseLogging();

        //Enable global exception handling
        app.UseExceptionHandler();

        //Add authentication and authorization middlewares
        app.UseAuthentication();

        app.UseAuthorization();

        //Map health check endpoints
        app.MapHealthCheckEndpoints();

        //Register all the endpoints discovered by Carter
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
