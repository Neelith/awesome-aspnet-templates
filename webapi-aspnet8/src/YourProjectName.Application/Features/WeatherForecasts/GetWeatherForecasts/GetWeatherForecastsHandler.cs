using YourProjectName.Application.Infrastructure.Caching;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IRedisCache redisCache)
    : IQueryHandler<GetWeatherForecastsQuery, GetWeatherForecastsResponse>
{
    public async Task<Result<GetWeatherForecastsResponse>> Handle(GetWeatherForecastsQuery? query, CancellationToken? cancellationToken = null)
    {
        const string cacheKey = "weatherforecasts";

        var cachedForecasts = await redisCache.GetAsync<List<WeatherForecast>>(cacheKey);

        if (cachedForecasts is not null)
        {
            return new GetWeatherForecastsResponse(cachedForecasts);
        }

        var forecasts = await weatherForecastRepository
            .GetWeatherForecasts(query?.TemperatureRangeMin, query?.TemperatureRangeMax);

        var response = new GetWeatherForecastsResponse(forecasts);

        await redisCache.SetAsync(cacheKey, forecasts, TimeSpan.FromMinutes(2));

        return response;
    }
}
