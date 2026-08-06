using Microsoft.Extensions.Caching.Hybrid;
using YourProjectName.Core.Abstractions.Persistence;
using YourProjectName.Core.Constants;
using YourProjectName.Core.Repositories.WeatherForecastRepository;
using YourProjectName.Core.Repositories.WeatherForecastRepository.Commands;

namespace YourProjectName.Core.Features.WeatherForecasts.CreateWeatherForecasts;

public sealed class CreateWeatherForecastCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork,
    HybridCache cache)
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

        await cache.RemoveByTagAsync(CacheTags.WeatherForecasts, cancellationToken);

        return IdResponse<int>.Create(createWeatherForecastResult.Value!.Id);
    }
}
