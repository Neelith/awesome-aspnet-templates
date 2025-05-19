using Microsoft.EntityFrameworkCore;
using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Infrastructure.Persistence.Repositories;
internal class WeatherForecastRepository(ApplicationDbContext applicationDbContext) : IWeatherForecastRepository
{
    public async Task<List<WeatherForecast>> GetWeatherForecasts(int? temperatureRangeMin, int? temperatureRangeMax)
    {
        var query = applicationDbContext.Forecasts.AsNoTracking();

        if (temperatureRangeMin.HasValue)
        {
            query = query.Where(x => x.TemperatureC >= temperatureRangeMin.Value);
        }

        if (temperatureRangeMax.HasValue)
        {
            query = query.Where(x => x.TemperatureC <= temperatureRangeMax.Value);
        }

        return await query.ToListAsync();
    }
}
