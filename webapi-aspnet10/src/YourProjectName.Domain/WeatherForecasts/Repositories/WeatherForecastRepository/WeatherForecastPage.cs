namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

public sealed record WeatherForecastPage(IReadOnlyList<WeatherForecast> Items, int TotalCount);
