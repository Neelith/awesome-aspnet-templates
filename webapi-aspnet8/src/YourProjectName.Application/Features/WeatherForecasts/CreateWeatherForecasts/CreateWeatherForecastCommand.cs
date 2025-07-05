using YourProjectName.Application.Infrastructure.Handlers;

namespace YourProjectName.Application.Features.WeatherForecasts.CreateWeatherForecasts;
public class CreateWeatherForecastCommand : ICommand<CreateWeatherForecastResponse>
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}
