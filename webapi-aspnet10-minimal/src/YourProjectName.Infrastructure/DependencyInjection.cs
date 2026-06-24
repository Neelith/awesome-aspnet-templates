using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .AddCaching(redisSettings, logger)
                .AddCurrentUserService();

        return services;
    }

    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        RedisSettings? redisSettings,
        ILogger logger)
    {
        //Register HybridCache with L2 Redis backend if configured
        //HybridCache provides L1 in-memory cache automatically
        if (redisSettings is not null && !string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;

                logger.LogInformation("Redis connection string {Url}", redisSettings.ConnectionString);

                options.InstanceName = redisSettings.KeyPrefix;
            });
        }
        else
        {
            logger.LogWarning("Redis settings not found or invalid. HybridCache will use L1 in-memory cache only");
        }

        services.AddHybridCache();

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
