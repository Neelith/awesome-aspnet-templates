namespace YourProjectName.Core.Entities.WeatherForecasts;

public static class WeatherForecastErrors
{
    public static Error NotFound(int id) => new(
        "WeatherForecast.NotFound",
        $"Weather forecast with id '{id}' was not found."
    );
}
