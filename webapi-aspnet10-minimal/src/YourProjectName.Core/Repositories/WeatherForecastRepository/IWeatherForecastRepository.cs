using Hermes.Results;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Core.Repositories.WeatherForecastRepository;

public interface IWeatherForecastRepository
{
    Task<Result<List<WeatherForecast>>> GetWeatherForecasts(GetWeatherForecastsRepositoryQuery? query, CancellationToken cancellationToken);
    Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken);
}
