using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using YourProjectName.Application.Infrastructure.Caching;

namespace YourProjectName.Infrastructure.Caching;
internal class RedisCache(IDistributedCache distributedCache) : IRedisCache
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await distributedCache.GetStringAsync(key, cancellationToken);
        return value is null ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(value);

        await distributedCache.SetStringAsync(key, json, options, cancellationToken);
    }
}
