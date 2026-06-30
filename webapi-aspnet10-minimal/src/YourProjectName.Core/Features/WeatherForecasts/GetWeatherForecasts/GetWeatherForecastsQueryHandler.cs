using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using YourProjectName.Core.Abstractions.Caching;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Repositories.WeatherForecastRepository;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Core.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryHandler(
    IWeatherForecastRepository weatherForecastRepository,
    HybridCache cache)
    : IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecast>>
{
    public async Task<Result<PagedResponse<WeatherForecast>>> Handle(GetWeatherForecastsQuery? query, CancellationToken cancellationToken)
    {
        const string cacheKey = "weatherforecasts";

        var parentActivity = Activity.Current;

        try
        {
            var forecasts = await cache.GetOrCreateAsync(
                cacheKey,
                async (CancellationToken ct) =>
                {
                    var previousActivity = Activity.Current;
                    try
                    {
                        Activity.Current = parentActivity;

                        var result = await weatherForecastRepository.GetWeatherForecasts(new GetWeatherForecastsRepositoryQuery
                        {
                            TemperatureRangeMin = query?.TemperatureRangeMin,
                            TemperatureRangeMax = query?.TemperatureRangeMax
                        }, ct);

                        if (result.IsFailure)
                        {
                            throw new CacheFactoryException(result.Errors, result.Metadata);
                        }

                        return result.Value;
                    }
                    finally
                    {
                        Activity.Current = previousActivity;
                    }
                },
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(2) },
                cancellationToken: cancellationToken);

            return PagedResponse<WeatherForecast>.Create(forecasts, forecasts.Count);
        }
        catch (CacheFactoryException ex)
        {
            return Result.Ko<PagedResponse<WeatherForecast>>(ex.Errors, ex.Metadata);
        }
    }
}
