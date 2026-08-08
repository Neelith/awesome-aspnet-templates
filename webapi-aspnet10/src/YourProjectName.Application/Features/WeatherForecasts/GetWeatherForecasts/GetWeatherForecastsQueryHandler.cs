using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IDistributedCache distributedCache)
    : IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecastResponse>>
{
    public async Task<Result<PagedResponse<WeatherForecastResponse>>> Handle(GetWeatherForecastsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"weatherforecasts:{query.TemperatureRangeMin}:{query.TemperatureRangeMax}:{query.PageNumber}:{query.PageSize}";

        var cachedValue = await distributedCache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedValue is not null)
        {
            var cachedResponse = JsonSerializer.Deserialize<PagedResponse<WeatherForecastResponse>>(cachedValue);
            if (cachedResponse is not null)
            {
                return cachedResponse;
            }
        }

        var page = await weatherForecastRepository.GetWeatherForecasts(new WeatherForecastFilter(
            query.TemperatureRangeMin,
            query.TemperatureRangeMax,
            query.PageNumber,
            query.PageSize), cancellationToken);

        var response = PagedResponse<WeatherForecastResponse>.Create(
            page.Items.Select(WeatherForecastResponse.FromDomain).ToList(),
            page.TotalCount);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };

        await distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options, cancellationToken);

        return response;
    }
}
