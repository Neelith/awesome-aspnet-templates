using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using YourProjectName.Infrastructure.Caching;
using YourProjectName.Infrastructure.HealthChecks;
using YourProjectName.Infrastructure.Persistence;
using YourProjectName.Infrastructure.Persistence.Repositories;
using YourProjectName.Infrastructure.Time;
using YourProjectName.Infrastructure.User;
using YourProjectName.Shared.Constants;

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
                .AddInfrastructureHealthChecks(redisSettings);

        return services;
    }

    private static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services, RedisSettings? redisSettings)
    {
        IHealthChecksBuilder builder = services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "postgresql",
                tags: [HealthCheckTags.Readiness]);

        if (redisSettings is not null && !string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            builder.AddCheck<RedisHealthCheck>(
                name: "redis",
                tags: [HealthCheckTags.Readiness]);
        }

        return services;
    }
}
