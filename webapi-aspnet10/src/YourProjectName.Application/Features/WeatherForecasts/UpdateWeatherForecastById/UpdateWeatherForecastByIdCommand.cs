namespace YourProjectName.Application.Features.WeatherForecasts.UpdateWeatherForecastById;

public class UpdateWeatherForecastByIdCommand : ICommand
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
