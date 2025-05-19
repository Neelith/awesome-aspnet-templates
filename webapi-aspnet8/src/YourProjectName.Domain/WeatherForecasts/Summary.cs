using System.Text.Json.Serialization;
using YourProjectName.Shared.Results;

namespace YourProjectName.Domain.WeatherForecasts;
public record Summary
{
    public string Value { get; private set; }

    [JsonConstructor]
    private Summary(string value)
    {
        Value = value;
    }

    public static Result<Summary> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Fail<Summary>(SummaryErrors.NullOrEmpty);
        }

        if (value.Length > 256)
        {
            return Result.Fail<Summary>(SummaryErrors.SummaryTooLong);
        }

        return Result.Ok(new Summary(value));
    }
}
