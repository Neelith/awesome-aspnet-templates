using Microsoft.EntityFrameworkCore;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Infrastructure.Persistence.Repositories;

internal class WeatherForecastRepository(ApplicationDbContext applicationDbContext) : IWeatherForecastRepository
{
    public async Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(command.Date);
        var weatherForecast = WeatherForecast.Create(date, command.TemperatureC, command.Summary);

        if (weatherForecast.IsFailure)
        {
            return Result.Ko<WeatherForecast>(weatherForecast.Errors, weatherForecast.Metadata);
        }

        await applicationDbContext.Forecasts.AddAsync(weatherForecast.Value, cancellationToken);

        return weatherForecast;
    }

    public async Task<Result<List<WeatherForecast>>> GetWeatherForecasts(GetWeatherForecastsRepositoryQuery? repositoryQuery, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.Forecasts.AsNoTracking();

        if (repositoryQuery is not null && repositoryQuery.TemperatureRangeMin.HasValue)
        {
            query = query.Where(x => x.TemperatureC >= repositoryQuery.TemperatureRangeMin.Value);
        }

        if (repositoryQuery is not null && repositoryQuery.TemperatureRangeMax.HasValue)
        {
            query = query.Where(x => x.TemperatureC <= repositoryQuery.TemperatureRangeMax.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
