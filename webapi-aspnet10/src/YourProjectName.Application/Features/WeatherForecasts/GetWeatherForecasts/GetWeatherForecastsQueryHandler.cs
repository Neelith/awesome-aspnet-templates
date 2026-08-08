using YourProjectName.Application.Infrastructure.Caching;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryHandler(
    IWeatherForecastRepository weatherForecastRepository,
    ICacheService cacheService)
    : IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecastResponse>>
{
    public async Task<Result<PagedResponse<WeatherForecastResponse>>> Handle(GetWeatherForecastsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"weatherforecasts:{query.TemperatureRangeMin}:{query.TemperatureRangeMax}:{query.PageNumber}:{query.PageSize}";

        var cachedResponse = await cacheService.GetAsync<PagedResponse<WeatherForecastResponse>>(cacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var page = await weatherForecastRepository.GetWeatherForecasts(new WeatherForecastFilter(
            query.TemperatureRangeMin,
            query.TemperatureRangeMax,
            query.PageNumber,
            query.PageSize), cancellationToken);

        var response = PagedResponse<WeatherForecastResponse>.Create(
            page.Items.Select(WeatherForecastResponse.FromDomain).ToList(),
            page.TotalCount);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2), cancellationToken);

        return response;
    }
}
