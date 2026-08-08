using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

namespace YourProjectName.Unit.Tests;

/// <summary>
/// Fix A — Replace custom cache abstraction (ICacheService) with IDistributedCache.
/// RED: handler currently depends on ICacheService, not IDistributedCache.
/// GREEN: handler constructor takes IDistributedCache; ICacheService + DistributedCacheService deleted.
/// </summary>
public sealed class FixA_CacheAbstractionTests
{
    private static readonly string RepoRootPath = RepoRoot.Find();

    /// <summary>
    /// Fix A (a): Handler must depend on IDistributedCache (not ICacheService).
    /// Current state: handler injects ICacheService → test fails RED.
    /// </summary>
    [Fact]
    public void GetWeatherForecastsQueryHandler_Constructor_HasIDistributedCacheParameter()
    {
        var handlerType = typeof(GetWeatherForecastsQueryHandler);
        var ctorParams = handlerType.GetConstructors()
            .SelectMany(c => c.GetParameters())
            .ToArray();

        var hasDistributedCache = ctorParams.Any(p => p.ParameterType == typeof(IDistributedCache));

        Assert.True(hasDistributedCache,
            $"Expected {nameof(GetWeatherForecastsQueryHandler)} constructor to have an " +
            $"{nameof(IDistributedCache)} parameter, but it does not. " +
            "Current parameters: " +
            string.Join(", ", ctorParams.Select(p => p.ParameterType.Name)));
    }

    /// <summary>
    /// Fix A (b): ICacheService type must NOT exist in the Application assembly.
    /// Current state: type exists → test fails RED.
    /// </summary>
    [Fact]
    public void ICacheService_Type_DoesNotExist()
    {
        var appAssembly = typeof(GetWeatherForecastsQueryHandler).Assembly;
        var cacheServiceType = appAssembly.GetType("YourProjectName.Application.Infrastructure.Caching.ICacheService");

        Assert.Null(cacheServiceType);
    }

    /// <summary>
    /// Fix A (b): DistributedCacheService.cs file must NOT exist.
    /// Current state: file exists → test fails RED.
    /// </summary>
    [Fact]
    public void DistributedCacheService_File_DoesNotExist()
    {
        var filePath = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
            "YourProjectName.Infrastructure", "Caching", "DistributedCacheService.cs");

        Assert.False(File.Exists(filePath),
            $"Expected file NOT to exist: {filePath}");
    }
}
