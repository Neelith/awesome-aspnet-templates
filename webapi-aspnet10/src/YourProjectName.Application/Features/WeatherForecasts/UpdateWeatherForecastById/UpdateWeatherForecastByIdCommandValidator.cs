namespace YourProjectName.Application.Features.WeatherForecasts.UpdateWeatherForecastById;

public sealed class UpdateWeatherForecastByIdCommandValidator : AbstractValidator<UpdateWeatherForecastByIdCommand>
{
    public UpdateWeatherForecastByIdCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
        RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100).WithMessage("Temperature must be between -100 and 100.");
        RuleFor(x => x.Summary).MaximumLength(256).WithMessage("Summary cannot exceed 256 characters.");
    }
}
