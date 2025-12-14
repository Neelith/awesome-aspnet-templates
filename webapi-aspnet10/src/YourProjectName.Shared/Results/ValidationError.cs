using FluentValidation.Results;

namespace YourProjectName.Shared.Results;

public sealed record ValidationError : Error
{
    public ValidationError(IEnumerable<Error> errors)
        : base(
            "Validation",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = [.. errors];
    }

    public ValidationError(IEnumerable<ValidationFailure> errors)
        : base(
            "Validation",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = [.. errors.Select(failure => new Error(failure.ErrorCode, failure.ErrorMessage, ErrorType.Validation))];
    }

    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
