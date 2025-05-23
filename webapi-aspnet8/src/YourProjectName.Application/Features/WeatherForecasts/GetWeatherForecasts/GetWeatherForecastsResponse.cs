using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Domain.WeatherForecasts;

namespace YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;

public sealed record GetWeatherForecastsResponse : IDataResponse<GetWeatherForecastsDataResponse>
{
    public GetWeatherForecastsDataResponse Data { get; init; }

    public GetWeatherForecastsResponse(IEnumerable<WeatherForecast> forecasts)
    {
        Data = new GetWeatherForecastsDataResponse(forecasts ?? []);
    }
};

public sealed record GetWeatherForecastsDataResponse(IEnumerable<WeatherForecast> Forecasts);