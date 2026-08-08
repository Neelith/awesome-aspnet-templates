using ResultExtensions = YourProjectName.Shared.Results.ResultExtensions;

namespace YourProjectName.Domain.WeatherForecasts;

public record Summary
{
    public const int MaxLength = 256;

    public string Value { get; private set; }

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

        if (value.Length > MaxLength)
        {
            return ResultExtensions.BadRequest<Summary>([SummaryErrors.SummaryTooLong]);
        }

        return Result.Ok(new Summary(value));
    }
}
