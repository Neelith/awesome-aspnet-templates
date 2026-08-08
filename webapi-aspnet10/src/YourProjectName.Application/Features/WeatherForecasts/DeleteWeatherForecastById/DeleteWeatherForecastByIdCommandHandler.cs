using YourProjectName.Application.Infrastructure.Persistence;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Shared.Constants;

namespace YourProjectName.Application.Features.WeatherForecasts.DeleteWeatherForecastById;

public sealed class DeleteWeatherForecastByIdCommandHandler(
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(DeleteWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        var existingForecast = await weatherForecastRepository.GetById(command.Id, cancellationToken);

        if (existingForecast is null)
        {
            return Result.Ko([new Error("404", "Weather forecast not found.")], new Dictionary<string, string?>
            {
                { ErrorConsts.ErrorType, ErrorConsts.NotFoundCode }
            });
        }

        existingForecast.MarkAsDeleted();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
