using YourProjectName.Application.Infrastructure.Persistence;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

namespace YourProjectName.Application.Features.WeatherForecasts.CreateWeatherForecasts;

public sealed class CreateWeatherForecastCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateWeatherForecastCommand, IdResponse<int>>
{
    public async Task<Result<IdResponse<int>>> Handle(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        var weatherForecastResult = WeatherForecast.Create(command.Date, command.TemperatureC, command.Summary);

        if (weatherForecastResult.IsFailure)
        {
            return Result.Ko<IdResponse<int>>(weatherForecastResult.Errors, weatherForecastResult.Metadata);
        }

        WeatherForecast weatherForecast = weatherForecastResult.Value!;

        await weatherForecastRepository.AddWeatherForecast(weatherForecast, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return IdResponse<int>.Create(weatherForecast.Id);
    }
}
