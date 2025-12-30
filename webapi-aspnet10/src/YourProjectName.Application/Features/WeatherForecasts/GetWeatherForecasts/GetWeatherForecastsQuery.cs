using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQuery : IQuery<PagedResponse<WeatherForecast>>
{
    public int? TemperatureRangeMin { get; set; }
    public int? TemperatureRangeMax { get; set; }
}
