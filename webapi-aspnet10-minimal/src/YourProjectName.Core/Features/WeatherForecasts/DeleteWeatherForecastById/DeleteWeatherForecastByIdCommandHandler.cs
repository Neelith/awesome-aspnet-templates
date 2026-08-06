using Microsoft.Extensions.Caching.Hybrid;
using YourProjectName.Core.Abstractions.Persistence;
using YourProjectName.Core.Constants;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Repositories.WeatherForecastRepository;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Queries;

namespace YourProjectName.Core.Features.WeatherForecasts.DeleteWeatherForecastById;

public sealed class DeleteWeatherForecastByIdCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork,
    HybridCache cache)
    : ICommandHandler<DeleteWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(DeleteWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        var forecastResult = await weatherForecastRepository.GetWeatherForecastById(
            new GetWeatherForecastByIdRepositoryQuery { Id = command.Id }, cancellationToken);

        if (forecastResult.IsFailure)
        {
            return Result.Ko(forecastResult.Errors, forecastResult.Metadata);
        }

        forecastResult.Value!.MarkAsDeleted();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.RemoveByTagAsync(CacheTags.WeatherForecasts, cancellationToken);

        return Result.Ok();
    }
}
