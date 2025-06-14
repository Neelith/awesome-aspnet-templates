using YourProjectName.Application.Infrastructure.Handlers;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQuery : IQuery<GetWeatherForecastsResponse>
{
    public int? TemperatureRangeMin { get; set; }
    public int? TemperatureRangeMax { get; set; }
}
