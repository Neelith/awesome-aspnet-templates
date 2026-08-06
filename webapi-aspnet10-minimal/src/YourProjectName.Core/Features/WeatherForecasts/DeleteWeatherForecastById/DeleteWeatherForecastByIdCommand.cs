namespace YourProjectName.Core.Features.WeatherForecasts.DeleteWeatherForecastById;

public class DeleteWeatherForecastByIdCommand : ICommand
{
    public int Id { get; set; }
}
