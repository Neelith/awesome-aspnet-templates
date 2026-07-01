using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YourProjectName.Core.Abstractions.Persistence;
using YourProjectName.Core.Services.Time;
using YourProjectName.Core.Services.User;
using YourProjectName.Infrastructure.Caching;
using YourProjectName.Infrastructure.HealthChecks;
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
                .AddCurrentUserService()
                .AddInfrastructureHealthChecks();

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        RedisSettings? redisSettings,
        ILogger logger)
    {
        //Register HybridCache with L2 Redis backend if configured
        //HybridCache provides L1 in-memory cache automatically
        if (redisSettings is not null && !string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            var opts = ConfigurationOptions.Parse(redisSettings.ConnectionString);
            opts.AbortOnConnectFail = false;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(opts));

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redisSettings.KeyPrefix;

                logger.LogInformation("Redis connection string {Url}", redisSettings.ConnectionString);
            });

            services.AddOptions<RedisCacheOptions>().Configure<IConnectionMultiplexer>((o, mux) =>
                o.ConnectionMultiplexerFactory = () => Task.FromResult(mux));
        }
        else
        {
            logger.LogWarning("Redis settings not found or invalid. HybridCache will use L1 in-memory cache only");
        }

        services.AddHybridCache();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        services.AddDbContext<ApplicationDbContext>((options) => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddTime(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        return services;
    }

    private static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    private static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services)
    {
        IHealthChecksBuilder builder = services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "postgresql",
                tags: ["ready"])
            .AddCheck<RedisHealthCheck>(
                name: "redis", 
                tags: ["ready"]);

        return services;
    }
}
