using System.Text.Json.Serialization;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Extensions;
using ResultExtensions = YourProjectName.Core.Extensions.ResultExtensions;

namespace YourProjectName.Core.ValueObjects;

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
            return ResultExtensions.BadRequest<Summary>([SummaryErrors.NullOrEmpty]);
        }

        if (value.Length > 256)
        {
            return ResultExtensions.BadRequest<Summary>([SummaryErrors.SummaryTooLong]);
        }

        return Hermes.Results.Result.Ok(new Summary(value));
    }
}
