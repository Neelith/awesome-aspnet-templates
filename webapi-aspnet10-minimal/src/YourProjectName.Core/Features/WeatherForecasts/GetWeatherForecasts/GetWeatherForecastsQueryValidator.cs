namespace YourProjectName.Core.Features.WeatherForecasts.GetWeatherForecasts;

public sealed class GetWeatherForecastsQueryValidator : AbstractValidator<GetWeatherForecastsQuery>
{
    public GetWeatherForecastsQueryValidator()
    {
        RuleFor(x => x.TemperatureRangeMin)
            .InclusiveBetween(-100, 100)
            .When(x => x.TemperatureRangeMin.HasValue);

        RuleFor(x => x.TemperatureRangeMax)
            .InclusiveBetween(-100, 100)
            .When(x => x.TemperatureRangeMax.HasValue);

        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
