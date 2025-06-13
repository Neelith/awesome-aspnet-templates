using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Application.Infrastructure.Persistance;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository.Commands;
using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Features.WeatherForecasts.CreateWeatherForecasts;
public sealed class CreateWeatherForecastCommandHandler(
    ILogger<CreateWeatherForecastCommandHandler> logger,
    IWeatherForecastRepository weatherForecastRepository,
    IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateWeatherForecastCommand, CreateWeatherForecastResponse>
{
    public async Task<Result<CreateWeatherForecastResponse>> Handle(CreateWeatherForecastCommand command, CancellationToken? cancellationToken = null)
    {
        var cancellationTokenValue = cancellationToken ?? CancellationToken.None;

        try
        {
            var createWeatherForecastResult = await weatherForecastRepository.CreateWeatherForecast(new CreateWeatherForecastRepositoryCommand
            {
                Date = command.Date,
                TemperatureC = command.TemperatureC,
                Summary = command.Summary
            });

            if (createWeatherForecastResult.IsFailure)
            {
                return Result.Fail<CreateWeatherForecastResponse>(createWeatherForecastResult.Error);
            }

            await unitOfWork.SaveChangesAsync(cancellationTokenValue);

            return new CreateWeatherForecastResponse
            {
                Id = createWeatherForecastResult.Value.Id
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while creating a weather forecast with {command}.", command);
            throw;
        }
    }
}
