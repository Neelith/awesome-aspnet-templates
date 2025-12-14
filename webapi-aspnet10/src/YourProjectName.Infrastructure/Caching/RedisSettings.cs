namespace YourProjectName.Infrastructure.Caching;
public record RedisSettings
{
    public string? ConnectionString { get; init; }
    public string? KeyPrefix { get; init; }
}
