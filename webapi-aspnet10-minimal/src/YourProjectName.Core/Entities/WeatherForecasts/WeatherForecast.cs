using System.Text.Json.Serialization;
using YourProjectName.Core.Shared;
using YourProjectName.Core.ValueObjects;

namespace YourProjectName.Core.Entities.WeatherForecasts;

public class WeatherForecast : AuditableEntity
{
    public int Id { get; private set; }
    public DateOnly Date { get; private set; }
    public int TemperatureC { get; private set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public Summary? Summary { get; private set; }

    private WeatherForecast(DateOnly date, int temperatureC, Summary? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    // Constructor used by EF Core materialization
    private WeatherForecast(int id, DateOnly date, int temperatureC) : this(date, temperatureC, null)
    {
        Id = id;
    }

    // Constructor used by System.Text.Json deserialization (HybridCache L2)
    [JsonConstructor]
    private WeatherForecast(int id, DateOnly date, int temperatureC, Summary? summary) : this(date, temperatureC, summary)
    {
        Id = id;
    }

    public static Result<WeatherForecast> Create(DateOnly date, int temperatureC, string? summaryValue)
    {
        Result<Summary>? summaryResult = CreateSummary(summaryValue);

        if (summaryResult?.IsFailure is true)
        {
            return Result.Ko<WeatherForecast>(summaryResult.Errors, summaryResult.Metadata);
        }

        return new WeatherForecast(date, temperatureC, summaryResult?.Value);
    }

    public Result Update(DateOnly date, int temperatureC, string? summaryValue)
    {
        Result<Summary>? summaryResult = CreateSummary(summaryValue);

        if (summaryResult?.IsFailure is true)
        {
            return Result.Ko(summaryResult.Errors, summaryResult.Metadata);
        }

        Date = date;
        TemperatureC = temperatureC;
        Summary = summaryResult?.Value;

        return Result.Ok();
    }

    private static Result<Summary>? CreateSummary(string? summaryValue)
    {
        return string.IsNullOrEmpty(summaryValue) ? null : Summary.Create(summaryValue!);
    }
}
