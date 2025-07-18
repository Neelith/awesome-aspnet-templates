﻿using System.Reflection;
using YourProjectName.Application;
using YourProjectName.Infrastructure;
using YourProjectName.Infrastructure.Caching;
using YourProjectName.Infrastructure.Persistence;
using YourProjectName.WebApi.Infrastructure.Settings;
using YourProjectName.WebApi.Infrastructure.Setup.Extensions;
using YourProjectName.WebApi.Infrastructure.Setup.Middlewares;

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
        string dbConnectionString = configuration.GetConnectionString("YourProjectNameDb")
            ?? throw new ApplicationException("Connection string 'YourProjectNameDb' not found.");

        //Add the redis settings to the container and get an istance of it
        RedisSettings? redisSettings = services.AddSettings<RedisSettings>(configuration, startupLogger)
            ?? throw new ApplicationException("Configuration section 'RedisSettings' not found.");

        //Add the jwt settings to the container and get an istance of it
        JwtSettings jwtSettings = services.AddSettings<JwtSettings>(configuration, startupLogger) 
            ?? throw new ApplicationException("Configuration section 'JwtSettings' not found.");

        //Register services here
        services
            .AddRouting(options => options.LowercaseUrls = true)
            .AddHttpContextAccessor()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .ConfigureProblemDetails()
            .AddAuthenticationServices(jwtSettings)
            .AddAuthorizationServices()
            .AddApplicationServices()
            .AddInfrastructureServices(startupLogger, dbConnectionString, redisSettings)
            .AddEndpoints(Assembly.GetExecutingAssembly())
            .AddOpenApiServices(jwtSettings);

        return services;
    }

    // Configure the HTTP request pipeline.
    public static void UseAppServices(this WebApplication app)
    {
        //Add x-trace header to all responses
        app.UseMiddleware<TraceMiddleware>();

        //Enable logging
        app.UseLogging();

        //Enable global exception handling
        app.UseExceptionHandler();

        //Add authentication and authorization middlewares
        app.UseAuthentication();

        app.UseAuthorization();

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
