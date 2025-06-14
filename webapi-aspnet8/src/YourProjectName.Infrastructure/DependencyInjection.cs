using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
}
