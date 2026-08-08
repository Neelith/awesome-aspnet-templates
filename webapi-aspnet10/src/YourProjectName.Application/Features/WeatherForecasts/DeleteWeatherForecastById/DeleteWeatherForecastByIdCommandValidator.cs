namespace YourProjectName.Application.Features.WeatherForecasts.DeleteWeatherForecastById;

public sealed class DeleteWeatherForecastByIdCommandValidator : AbstractValidator<DeleteWeatherForecastByIdCommand>
{
    public DeleteWeatherForecastByIdCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}
