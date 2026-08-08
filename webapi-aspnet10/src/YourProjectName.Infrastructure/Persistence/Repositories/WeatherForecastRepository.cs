using Microsoft.EntityFrameworkCore;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

namespace YourProjectName.Infrastructure.Persistence.Repositories;

internal class WeatherForecastRepository(ApplicationDbContext applicationDbContext) : IWeatherForecastRepository
{
    public async Task AddWeatherForecast(WeatherForecast weatherForecast, CancellationToken cancellationToken)
    {
        await applicationDbContext.Forecasts.AddAsync(weatherForecast, cancellationToken);
    }

    public async Task<WeatherForecastPage> GetWeatherForecasts(WeatherForecastFilter filter, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, filter.PageNumber);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var query = applicationDbContext.Forecasts.AsNoTracking();

        if (filter.TemperatureRangeMin.HasValue)
        {
            query = query.Where(x => x.TemperatureC >= filter.TemperatureRangeMin.Value);
        }

        if (filter.TemperatureRangeMax.HasValue)
        {
            query = query.Where(x => x.TemperatureC <= filter.TemperatureRangeMax.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        List<WeatherForecast> items = await query
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new WeatherForecastPage(items, totalCount);
    }
}
