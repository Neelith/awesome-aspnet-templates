using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed record GetWeatherForecastsResponse() : DataResponse<GetWeatherForecastsDataResponse>
{
    public static GetWeatherForecastsResponse Create(IEnumerable<WeatherForecast> forecasts)
        => Create<GetWeatherForecastsResponse>(new GetWeatherForecastsDataResponse(forecasts));
};

public sealed record GetWeatherForecastsDataResponse(IEnumerable<WeatherForecast> Forecasts);