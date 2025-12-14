using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Features.WeatherForecasts.UpdateWeatherForecastById;

public sealed class UpdateWeatherForecastByIdCommandHandler()
    : ICommandHandler<UpdateWeatherForecastByIdCommand>
{
    public async Task<Result> Handle(UpdateWeatherForecastByIdCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
