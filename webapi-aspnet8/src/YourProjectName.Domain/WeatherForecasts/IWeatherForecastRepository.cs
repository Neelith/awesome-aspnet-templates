namespace YourProjectName.Domain.WeatherForecasts;
public interface IWeatherForecastRepository
{
    Task<List<WeatherForecast>> GetWeatherForecasts(int? temperatureRangeMin, int? temperatureRangeMax);
}
