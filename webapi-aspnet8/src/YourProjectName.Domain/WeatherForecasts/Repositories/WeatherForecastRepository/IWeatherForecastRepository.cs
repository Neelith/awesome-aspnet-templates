using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Shared.Results;

namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
public interface IWeatherForecastRepository
{
    Task<List<WeatherForecast>> GetWeatherForecasts(int? temperatureRangeMin, int? temperatureRangeMax);
    Task<Result<WeatherForecast>> CreateWeatherForecast(CreateWeatherForecastRepositoryCommand command);
}
