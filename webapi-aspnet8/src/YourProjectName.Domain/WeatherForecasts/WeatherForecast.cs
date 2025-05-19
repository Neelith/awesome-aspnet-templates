using System.Text.Json.Serialization;
using YourProjectName.Shared.Domain;
using YourProjectName.Shared.Results;

namespace YourProjectName.Domain.WeatherForecasts;

public class WeatherForecast : Entity
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

    [JsonConstructor]
    private WeatherForecast(int id, DateOnly date, int temperatureC, Summary? summary) : this(date, temperatureC, summary)
    {
        Id = id;
    }

    public static Result<WeatherForecast> Create(DateOnly date, int temperatureC, string? summaryValue)
    {
        bool isSummaryValorized = !string.IsNullOrEmpty(summaryValue);

        var summaryCreationResult = isSummaryValorized ? Summary.Create(summaryValue!) : null;

        if (summaryCreationResult is { IsFailure: true })
        {
            return Result.Fail<WeatherForecast>(summaryCreationResult.Error);
        }

        return new WeatherForecast(date, temperatureC, isSummaryValorized ? summaryCreationResult!.Value : null);
    }
}
