namespace YourProjectName.Core.Features.WeatherForecasts.CreateWeatherForecasts;

public class CreateWeatherForecastCommandValidator : AbstractValidator<CreateWeatherForecastCommand>
{
    public CreateWeatherForecastCommandValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
        RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100).WithMessage("Temperature must be between -100 and 100.");
        RuleFor(x => x.Summary).MaximumLength(256).WithMessage("Summary cannot exceed 256 characters.");
    }
}
