using FluentValidation;

namespace YourProjectName.Application.Features.WeatherForecasts.UpdateWeatherForecastById;

public sealed class UpdateWeatherForecastByIdCommandValidator : AbstractValidator<UpdateWeatherForecastByIdCommand>
{
    public UpdateWeatherForecastByIdCommandValidator()
    {
    }
}
