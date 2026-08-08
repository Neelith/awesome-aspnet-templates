using YourProjectName.Application.Infrastructure.Persistence;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Shared.Constants;

namespace YourProjectName.Application.Features.WeatherForecasts.UpdateWeatherForecastById;

public sealed class UpdateWeatherForecastByIdCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(UpdateWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        var existingForecast = await weatherForecastRepository.GetById(command.Id, cancellationToken);

        if (existingForecast is null)
        {
            return Result.Ko([new Error("404", "Weather forecast not found.")], new Dictionary<string, string?>
            {
                { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode }
            });
        }

        var updateResult = existingForecast.Update(command.Date, command.TemperatureC, command.Summary);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
