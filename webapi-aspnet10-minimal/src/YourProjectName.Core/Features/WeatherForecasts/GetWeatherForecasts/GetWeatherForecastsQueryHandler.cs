using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using YourProjectName.Core.Abstractions.Caching;
using YourProjectName.Core.Abstractions.Diagnostics;
using YourProjectName.Core.Constants;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Repositories.WeatherForecastRepository;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Core.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryHandler(
    IWeatherForecastRepository weatherForecastRepository,
    HybridCache cache)
    : IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecast>>
{
    public async Task<Result<PagedResponse<WeatherForecast>>> Handle(GetWeatherForecastsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"weatherforecasts:{query.TemperatureRangeMin}:{query.TemperatureRangeMax}:{query.PageNumber}:{query.PageSize}";

        // HybridCache can run the factory on a detached execution context where Activity.Current is lost;
        // the parent context is captured here and re-attached explicitly inside the factory.
        ActivityContext? parentContext = Activity.Current?.Context;

        try
        {
            PagedResponse<WeatherForecast> forecasts = await cache.GetOrCreateAsync(
                cacheKey,
                cancellationToken => FetchPage(query, parentContext, cancellationToken),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(2) },
                tags: [CacheTags.WeatherForecasts],
                cancellationToken: cancellationToken);

            return Result.Ok(forecasts);
        }
        catch (CacheFactoryException ex)
        {
            return Result.Ko<PagedResponse<WeatherForecast>>(ex.Errors, ex.Metadata);
        }
    }

    private async ValueTask<PagedResponse<WeatherForecast>> FetchPage(
        GetWeatherForecastsQuery query,
        ActivityContext? parentContext,
        CancellationToken cancellationToken)
    {
        using Activity? activity = ApplicationDiagnostics.ActivitySource.StartActivity(
            "WeatherForecastsCacheFactory",
            ActivityKind.Internal,
            parentContext ?? default);

        var result = await weatherForecastRepository.GetWeatherForecasts(new GetWeatherForecastsRepositoryQuery
        {
            TemperatureRangeMin = query.TemperatureRangeMin,
            TemperatureRangeMax = query.TemperatureRangeMax,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        }, cancellationToken);

        if (result.IsFailure)
        {
            throw new CacheFactoryException(result.Errors, result.Metadata);
        }

        return result.Value!;
    }
}
