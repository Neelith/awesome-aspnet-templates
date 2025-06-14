using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Shared.Results;

namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
public interface IWeatherForecastRepository
{
    Task<List<WeatherForecast>> GetWeatherForecasts(int? temperatureRangeMin, int? temperatureRangeMax, CancellationToken cancellationToken);
    Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command, CancellationToken cancellationToken);
}
