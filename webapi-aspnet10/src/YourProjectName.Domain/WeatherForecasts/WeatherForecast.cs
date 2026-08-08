using YourProjectName.Domain.Shared;

namespace YourProjectName.Domain.WeatherForecasts;

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

    //EF constructor
    private WeatherForecast(int id, DateOnly date, int temperatureC) : this(date, temperatureC, null)
    {
        Id = id;
    }

    public static Result<WeatherForecast> Create(DateOnly date, int temperatureC, string? summaryValue)
    {
        bool isSummaryValorized = !string.IsNullOrEmpty(summaryValue);

        var summaryCreationResult = isSummaryValorized ? Summary.Create(summaryValue!) : null;

        if (summaryCreationResult?.IsFailure is true)
        {
            return Result.Ko<WeatherForecast>(summaryCreationResult.Errors, summaryCreationResult.Metadata);
        }

        return new WeatherForecast(date, temperatureC, isSummaryValorized ? summaryCreationResult!.Value : null);
    }

    public Result Update(DateOnly date, int temperatureC, string? summaryValue)
    {
        bool isSummaryValorized = !string.IsNullOrEmpty(summaryValue);

        var summaryCreationResult = isSummaryValorized ? Summary.Create(summaryValue!) : null;

        if (summaryCreationResult?.IsFailure is true)
        {
            return Result.Ko(summaryCreationResult.Errors, summaryCreationResult.Metadata);
        }

        Date = date;
        TemperatureC = temperatureC;
        Summary = isSummaryValorized ? summaryCreationResult!.Value : null;

        return Result.Ok();
    }
}
