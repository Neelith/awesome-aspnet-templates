using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Application.Infrastructure.Caching;

namespace YourProjectName.Infrastructure.Caching;
internal static class AddRedisExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services, RedisSettings? redisSettings)
    {
        //Add redis only if we have a proper connection string configured
        //Otherwise, use the in-memory cache
        if (redisSettings is null || string.IsNullOrEmpty(redisSettings.ConnectionString))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = redisSettings.KeyPrefix;
            });
        }

        services.AddSingleton<IRedisCache, RedisCache>();

        return services;
    }
}
