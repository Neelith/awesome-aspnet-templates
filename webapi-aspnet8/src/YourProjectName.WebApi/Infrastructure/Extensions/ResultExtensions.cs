using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using YourProjectName.Shared.Results;

namespace YourProjectName.WebApi.Infrastructure.Extensions;

internal static class ResultExtensions
{
    public static IResult Match<T>(
        this Result<T> result,
        Func<Result<T>, IResult> onSuccess,
        Func<Result<T>, IResult> onFailure)
    {
        if (result.IsFailure)
        {
            return onFailure(result);
        }

        return onSuccess(result);
    }

    public static IResult Match(
        this Result result,
        Func<Result, IResult> onSuccess,
        Func<Result, IResult> onFailure)
    {
        if (result.IsFailure)
        {
            return onFailure(result);
        }

        return onSuccess(result);
    }

    public static ProblemHttpResult ToErrorResponse<T>(this Result<T> result)
    {
        return ToErrorResponse(result as Result);
    }

    public static ProblemHttpResult ToErrorResponse(this Result result)
    {
        if (result is null || result.IsSuccess)
        {
            throw new ArgumentException("Expected 'failed' result, but 'success' result was found instead");
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => result.ToProblem(ErrorType.Validation, HttpStatusCode.BadRequest),
            ErrorType.NotFound => result.ToProblem(ErrorType.NotFound, HttpStatusCode.NotFound),
            ErrorType.Problem => result.ToProblem(ErrorType.Problem, HttpStatusCode.InternalServerError),
            _ => throw new ArgumentException("Unhandled result error code"),
        };
    }

    private static ProblemHttpResult ToProblem(this Result result, ErrorType errorType, HttpStatusCode statusCode)
    {

        if (result.Error.Type != errorType)
        {
            throw new ArgumentException($"Expected '{errorType}' but '{result.Error.Type} was found instead'");
        }

        var errors = result.Error is ValidationError validationError
            ? string.Join(", ", validationError.Errors.Select(e => e.Description))
            : result.Error.Description;

        return TypedResults.Problem(errors, statusCode: (int)statusCode);
    }
}
