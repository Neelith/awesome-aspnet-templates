using Carter;
using Hermes.Responses;
using Microsoft.AspNetCore.Mvc;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Features.WeatherForecasts.CreateWeatherForecasts;
using YourProjectName.Core.Features.WeatherForecasts.DeleteWeatherForecastById;
using YourProjectName.Core.Features.WeatherForecasts.GetWeatherForecasts;
using YourProjectName.Core.Features.WeatherForecasts.UpdateWeatherForecastById;
using YourProjectName.WebApi.Constants;
using YourProjectName.WebApi.Extensions;

namespace YourProjectName.WebApi.Endpoints.WeatherForecasts;

public class WeatherForecastsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("weatherforecasts")
            .WithTags(Tags.WeatherForecast)
            .WithDescription("Weather forecast endpoints");

        group.MapGet("", async
            ([AsParameters] GetWeatherForecastsQuery query,
            [FromServices] IQueryHandler<GetWeatherForecastsQuery, PagedResponse<WeatherForecast>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);

            IResult response = result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : result.ToErrorResponse();

            return response;
        })
        .WithDescription("Retrieves a paginated list of weather forecasts based on the provided query parameters.")
        .Produces<PagedResponse<WeatherForecast>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        group.MapPost("", async
            ([FromBody] CreateWeatherForecastCommand command,
            [FromServices] ICommandHandler<CreateWeatherForecastCommand, IdResponse<int>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            IResult response = result.IsSuccess
                ? TypedResults.Created($"weatherforecasts/{result.Value?.Data.Id}", result.Value)
                : result.ToErrorResponse();

            return response;
        })
        .WithDescription("Creates a new weather forecast with the provided details.")
        .Produces<IdResponse<int>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        group.MapPut("{id:int}", async
            ([FromRoute] int id,
            [FromBody] UpdateWeatherForecastByIdCommand body,
            [FromServices] ICommandHandler<UpdateWeatherForecastByIdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateWeatherForecastByIdCommand
            {
                Id = id,
                Date = body.Date,
                TemperatureC = body.TemperatureC,
                Summary = body.Summary
            };

            var result = await handler.Handle(command, cancellationToken);

            IResult response = result.IsSuccess
                ? TypedResults.NoContent()
                : result.ToErrorResponse();

            return response;
        })
        .WithDescription("Updates an existing weather forecast by id.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();

        group.MapDelete("{id:int}", async
            ([FromRoute] int id,
            [FromServices] ICommandHandler<DeleteWeatherForecastByIdCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new DeleteWeatherForecastByIdCommand { Id = id }, cancellationToken);

            IResult response = result.IsSuccess
                ? TypedResults.NoContent()
                : result.ToErrorResponse();

            return response;
        })
        .WithDescription("Soft-deletes an existing weather forecast by id.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .RequireAuthorization();
    }
}
