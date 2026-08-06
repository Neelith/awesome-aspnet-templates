using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using YourProjectName.Core.Constants;

namespace YourProjectName.WebApi.Extensions;

internal static class ResultExtensions
{
    private const string GenericErrorDetail = "An unexpected error occurred.";

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

        string? errorType = default;

        var errorTypeParsed = result.Metadata?.TryGetValue(ErrorConsts.ErrorType, out errorType);

        if (errorTypeParsed is not true || errorType is null)
        {
            return result.ToProblem(HttpStatusCode.InternalServerError);
        }

        return errorType switch
        {
            ErrorConsts.BadRequestCode => result.ToProblem(HttpStatusCode.BadRequest),
            ErrorConsts.UnauthorizedCode => result.ToProblem(HttpStatusCode.Unauthorized),
            ErrorConsts.NotFoundCode => result.ToProblem(HttpStatusCode.NotFound),
            ErrorConsts.InternalServerErrorCode => result.ToProblem(HttpStatusCode.InternalServerError),
            _ => throw new ArgumentException("Unhandled result error code"),
        };
    }

    private static ProblemHttpResult ToProblem(this Result result, HttpStatusCode statusCode)
    {
        //Internal server errors must not leak implementation details to the client
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            return TypedResults.Problem(detail: GenericErrorDetail, statusCode: (int)statusCode, title: statusCode.ToString());
        }

        var errors = result.Errors.Count > 1
            ? string.Join("\n---\n", result.Errors.Select(e => e.Message))
            : result.Errors.Count == 0 ? GenericErrorDetail : result.Errors[0].Message;

        return TypedResults.Problem(detail: errors, statusCode: (int)statusCode, title: statusCode.ToString());
    }
}
