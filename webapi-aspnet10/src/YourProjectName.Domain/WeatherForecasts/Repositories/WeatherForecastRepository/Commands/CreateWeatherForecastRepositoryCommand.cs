namespace YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;

public class CreateWeatherForecastRepositoryCommand
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
