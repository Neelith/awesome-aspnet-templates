using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed record WeatherForecastResponse(
    int Id,
    DateOnly Date,
    int TemperatureC,
    int TemperatureF,
    string? Summary)
{
    public static WeatherForecastResponse FromDomain(WeatherForecast weatherForecast) =>
        new(weatherForecast.Id,
            weatherForecast.Date,
            weatherForecast.TemperatureC,
            weatherForecast.TemperatureF,
            weatherForecast.Summary?.Value);
}
