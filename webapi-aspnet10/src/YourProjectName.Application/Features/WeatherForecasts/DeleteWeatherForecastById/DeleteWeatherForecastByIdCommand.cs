namespace YourProjectName.Application.Features.WeatherForecasts.DeleteWeatherForecastById;

public class DeleteWeatherForecastByIdCommand : ICommand
{
    public int Id { get; set; }
}
