using Hermes.Results;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

public interface IWeatherForecastRepository
{
    Task<Result<List<WeatherForecast>>> GetWeatherForecasts(GetWeatherForecastsRepositoryQuery? query, CancellationToken cancellationToken);
    Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken);
}
