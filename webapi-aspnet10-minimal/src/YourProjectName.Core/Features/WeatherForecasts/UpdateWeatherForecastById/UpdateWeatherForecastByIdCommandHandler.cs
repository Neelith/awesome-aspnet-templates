using Microsoft.Extensions.Caching.Hybrid;
using YourProjectName.Core.Abstractions.Persistence;
using YourProjectName.Core.Constants;
using YourProjectName.Core.Repositories.WeatherForecastRepository;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Core.Features.WeatherForecasts.UpdateWeatherForecastById;

public sealed class UpdateWeatherForecastByIdCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork,
    HybridCache cache)
    : ICommandHandler<UpdateWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(UpdateWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        var forecastResult = await weatherForecastRepository.GetWeatherForecastById(
            new GetWeatherForecastByIdRepositoryQuery { Id = command.Id }, cancellationToken);

        if (forecastResult.IsFailure)
        {
            return Result.Ko(forecastResult.Errors, forecastResult.Metadata);
        }

        var updateResult = forecastResult.Value!.Update(command.Date, command.TemperatureC, command.Summary);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(CacheTags.WeatherForecasts, cancellationToken);

        return Result.Ok();
    }
}
