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

        group.MapGet("/secured", async
            ([AsParameters] GetWeatherForecastsQuery query,
            [FromServices] IQueryHandler<GetWeatherForecastsQuery, GetWeatherForecastsResponse> handler,
            [FromServices] IHttpContextAccessor contextAccessor) =>
        {
            var user = contextAccessor?.HttpContext?.User;
            var result = await handler.Handle(query);

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
            var result = await handler.Handle(command);

            return result.Match(
                result => TypedResults.Created($"weatherforecasts/{result.Value.Id}", result.Value),
                result => result.ToErrorResponse()
            );
        })
            .Produces<CreateWeatherForecastResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

        group.MapGet("", async
            ([AsParameters] GetWeatherForecastsQuery query,
            [FromServices] IQueryHandler<GetWeatherForecastsQuery, GetWeatherForecastsResponse> handler) =>
            {
                var result = await handler.Handle(query);

                return result.Match(
                    result => TypedResults.Ok(result.Value),
                    result => result.ToErrorResponse()
                );
            })
            .Produces<GetWeatherForecastsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

    }
}