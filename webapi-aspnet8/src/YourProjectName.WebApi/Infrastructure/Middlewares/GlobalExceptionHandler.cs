using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using YourProjectName.WebApi.Constants;

namespace YourProjectName.WebApi.Infrastructure.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");

        bool isTraceHeaderPresent = httpContext.Request.Headers.TryGetValue(Headers.Trace, out StringValues traceHeaderValue);

        string traceId = isTraceHeaderPresent && !StringValues.IsNullOrEmpty(traceHeaderValue) ? traceHeaderValue.ToString() : httpContext.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure"
        };

        problemDetails.Extensions.Add("traceId", traceId);

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}