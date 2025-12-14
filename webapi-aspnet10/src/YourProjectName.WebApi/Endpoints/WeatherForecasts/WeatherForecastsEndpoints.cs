using Microsoft.AspNetCore.Mvc;
using YourProjectName.Application.Features.WeatherForecasts.CreateWeatherForecasts;
using YourProjectName.Application.Features.WeatherForecasts.GetWeatherForecasts;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Shared.Results;
using YourProjectName.WebApi.Constants;
using YourProjectName.WebApi.Infrastructure.Extensions;

namespace YourProjectName.WebApi.Endpoints.WeatherForecasts;

public class WeatherForecastsEndpoints : IEndpoints
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("weatherforecasts")
            .WithTags(Tags.WeatherForecast)
            .WithDescription("Weather forecast endpoints");

        group.MapGet("", async
            ([AsParameters] GetWeatherForecastsQuery query,
            [FromServices] IQueryHandler<GetWeatherForecastsQuery, GetWeatherForecastsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                result => TypedResults.Ok(result.Value),
                result => result.ToErrorResponse()
            );
        })
        .Produces<GetWeatherForecastsResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        group.MapPost("", async
            ([FromBody] CreateWeatherForecastCommand command, 
            [FromServices] ICommandHandler<CreateWeatherForecastCommand, CreateWeatherForecastResponse> handler) =>
        {
            var result = await handler.Handle(command, CancellationToken.None);

            return result.Match(
                result => TypedResults.Created($"weatherforecasts/{result.Value.Id}", result.Value),
                result => result.ToErrorResponse()
            );
        })
        .Produces<CreateWeatherForecastResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}