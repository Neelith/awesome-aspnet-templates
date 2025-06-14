using Microsoft.EntityFrameworkCore;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Shared.Results;

namespace YourProjectName.Infrastructure.Persistence.Repositories;
internal class WeatherForecastRepository(ApplicationDbContext applicationDbContext) : IWeatherForecastRepository
{
    public async Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(command.Date);
        var weatherForecast = WeatherForecast.Create(date, command.TemperatureC, command.Summary);

        if (weatherForecast.IsFailure)
        {
            return Result.Fail<WeatherForecast>(weatherForecast.Error);
        }

        await applicationDbContext.Forecasts.AddAsync(weatherForecast.Value, cancellationToken);

        return weatherForecast;
    }

    public async Task<List<WeatherForecast>> GetWeatherForecasts(int? temperatureRangeMin, int? temperatureRangeMax, CancellationToken cancellationToken)
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

        return await query.ToListAsync(cancellationToken);
    }
}
