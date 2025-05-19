using YourProjectName.Infrastructure.Caching;

namespace YourProjectName.WebApi.Infrastructure.Setup;

internal static class AddRedisSettingsExtension
{

    public static RedisSettings? AddRedisSettings(this IServiceCollection services, IConfiguration configuration)
    {
        //Get the redis settings from the configuration
        var redisSettingsSection = configuration.GetSection(nameof(RedisSettings));

        if (redisSettingsSection is null)
        {
            return null;
        }

        //Add the redis settings to the DI container
        services.Configure<RedisSettings>(redisSettingsSection);

        //Return the redis settings
        return redisSettingsSection.Get<RedisSettings>();
    }
}