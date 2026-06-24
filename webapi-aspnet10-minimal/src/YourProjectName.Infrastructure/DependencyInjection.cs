using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YourProjectName.Core.Abstractions.Caching;
using YourProjectName.Core.Abstractions.Persistence;
using YourProjectName.Core.Services.Time;
using YourProjectName.Core.Services.User;
using YourProjectName.Infrastructure.Caching;
using YourProjectName.Infrastructure.Persistence;
using YourProjectName.Infrastructure.Persistence.Repositories;
using YourProjectName.Infrastructure.Time;
using YourProjectName.Infrastructure.User;

namespace YourProjectName.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        ILogger logger,
        string? dbConnectionString,
        RedisSettings? redisSettings = default)
    {
        //Register infrastructure services here
        ArgumentNullException.ThrowIfNull(dbConnectionString, nameof(dbConnectionString));

        services.AddTime()
                .AddDbContext(dbConnectionString)
                .AddRepositories()
                .AddRedis(redisSettings, logger)
                .AddCurrentUserService();

        return services;
    }

    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        RedisSettings? redisSettings,
        ILogger logger)
    {
        //Add redis only if we have a proper connection string configured
        //Otherwise, use the in-memory cache
        if (redisSettings is null || string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            logger.LogWarning("Redis settings not found or invalid. Using in-memory cache");
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;

                logger.LogInformation("Redis connection string {Url}", redisSettings.ConnectionString);

                options.InstanceName = redisSettings.KeyPrefix;
            });
        }

        services.AddSingleton<IRedisCache, RedisCache>();

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        services.AddDbContext<ApplicationDbContext>((options) => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static IServiceCollection AddTime(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        return services;
    }

    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
