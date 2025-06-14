using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Queries;
using YourProjectName.Shared.Results;

namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
public interface IWeatherForecastRepository
{
    Task<List<WeatherForecast>> GetWeatherForecasts(GetWeatherForecastsRepositoryQuery? query, CancellationToken cancellationToken);
    Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken);
}
