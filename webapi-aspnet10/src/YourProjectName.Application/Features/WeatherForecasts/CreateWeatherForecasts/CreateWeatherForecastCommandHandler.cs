using YourProjectName.Application.Infrastructure.Persistence;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;

namespace YourProjectName.Application.Features.WeatherForecasts.CreateWeatherForecasts;

public sealed class CreateWeatherForecastCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateWeatherForecastCommand, IdResponse<int>>
{
    public async Task<Result<IdResponse<int>>> Handle(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        var createWeatherForecastResult = await weatherForecastRepository.CreateWeatherForecast(new CreateWeatherForecastRepositoryCommand
        {
            Date = command.Date,
            TemperatureC = command.TemperatureC,
            Summary = command.Summary
        }, cancellationToken);

        if (createWeatherForecastResult.IsFailure)
        {
            return Result.Ko<IdResponse<int>>(createWeatherForecastResult.Errors, createWeatherForecastResult.Metadata);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return IdResponse<int>.Create(createWeatherForecastResult.Value!.Id);
    }
}
