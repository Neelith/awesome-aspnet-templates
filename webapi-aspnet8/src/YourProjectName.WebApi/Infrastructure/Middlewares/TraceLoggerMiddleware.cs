
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using YourProjectName.WebApi.Constants;

namespace YourProjectName.WebApi.Infrastructure.Middlewares;

public class TraceLoggerMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        bool isTraceHeaderPresent = context.Request.Headers.TryGetValue(Headers.Trace, out StringValues traceHeaderValue);

        string traceId = isTraceHeaderPresent && !StringValues.IsNullOrEmpty(traceHeaderValue) ? traceHeaderValue.ToString() : context.TraceIdentifier;

        using (LogContext.PushProperty("TraceId", traceId))
        {
            return next(context);
        }
    }
}
