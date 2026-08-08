namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

public interface IWeatherForecastRepository
{
    Task<WeatherForecastPage> GetWeatherForecasts(WeatherForecastFilter filter, CancellationToken cancellationToken);
    Task AddWeatherForecast(WeatherForecast weatherForecast, CancellationToken cancellationToken);
}
