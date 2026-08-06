namespace YourProjectName.Core.Features.WeatherForecasts.CreateWeatherForecasts;

public class CreateWeatherForecastCommand : ICommand<IdResponse<int>>
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
