using Hermes.Responses;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Repositories.WeatherForecastRepository;
using ResultExtensions = YourProjectName.Core.Extensions.ResultExtensions;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Infrastructure.Persistence.Repositories;

internal class WeatherForecastRepository(ApplicationDbContext applicationDbContext) : IWeatherForecastRepository
{
    public async Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken)
    {
        var weatherForecast = WeatherForecast.Create(command.Date, command.TemperatureC, command.Summary);

        if (weatherForecast.IsFailure)
        {
            return Result.Ko<WeatherForecast>(weatherForecast.Errors, weatherForecast.Metadata);
        }

        await applicationDbContext.Forecasts.AddAsync(weatherForecast.Value!, cancellationToken);

        return weatherForecast;
    }

    public async Task<Result<WeatherForecast>> GetWeatherForecastById(GetWeatherForecastByIdRepositoryQuery query, CancellationToken cancellationToken)
    {
        WeatherForecast? forecast = await applicationDbContext.Forecasts
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

        if (forecast is null)
        {
            return ResultExtensions.NotFound<WeatherForecast>([WeatherForecastErrors.NotFound(query.Id)]);
        }

        return forecast;
    }

    public async Task<Result<PagedResponse<WeatherForecast>>> GetWeatherForecasts(GetWeatherForecastsRepositoryQuery repositoryQuery, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, repositoryQuery.PageNumber);
        var pageSize = Math.Clamp(repositoryQuery.PageSize, 1, 100);

        var query = applicationDbContext.Forecasts.AsNoTracking();

        if (repositoryQuery.TemperatureRangeMin.HasValue)
        {
            query = query.Where(x => x.TemperatureC >= repositoryQuery.TemperatureRangeMin.Value);
        }

        if (repositoryQuery.TemperatureRangeMax.HasValue)
        {
            query = query.Where(x => x.TemperatureC <= repositoryQuery.TemperatureRangeMax.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        List<WeatherForecast> items = await query
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<WeatherForecast>.Create(items, totalCount);
    }
}
