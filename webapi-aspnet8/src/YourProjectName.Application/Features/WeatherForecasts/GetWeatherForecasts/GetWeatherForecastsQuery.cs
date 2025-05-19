using YourProjectName.Application.Infrastructure.Handlers;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed record GetWeatherForecastsQuery(
    int? TemperatureRangeMin,
    int? TemperatureRangeMax)
    : IQuery<GetWeatherForecastsResponse>;
