namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

public sealed record WeatherForecastFilter(
    int? TemperatureRangeMin,
    int? TemperatureRangeMax,
    int PageNumber = 1,
    int PageSize = 20);
