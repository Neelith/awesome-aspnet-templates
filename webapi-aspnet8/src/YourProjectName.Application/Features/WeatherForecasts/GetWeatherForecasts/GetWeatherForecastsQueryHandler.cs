using YourProjectName.Application.Infrastructure.Caching;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IRedisCache redisCache)
    : IQueryHandler<GetWeatherForecastsQuery, GetWeatherForecastsResponse>
{
    public async Task<Result<GetWeatherForecastsResponse>> Handle(GetWeatherForecastsQuery? query, CancellationToken cancellationToken)
    {
        const string cacheKey = "weatherforecasts";

        var cachedForecasts = await redisCache.GetAsync<List<WeatherForecast>>(cacheKey, cancellationToken);

        if (cachedForecasts is not null)
        {
            return new GetWeatherForecastsResponse(cachedForecasts);
        }

        var forecasts = await weatherForecastRepository
            .GetWeatherForecasts(query?.TemperatureRangeMin, query?.TemperatureRangeMax, cancellationToken);

        var response = new GetWeatherForecastsResponse(forecasts);

        await redisCache.SetAsync(cacheKey, forecasts, TimeSpan.FromMinutes(2), cancellationToken);

        return response;
    }
}
