using Serilog.Context;
using YourProjectName.WebApi.Constants;

namespace YourProjectName.WebApi.Infrastructure.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string traceId = context.Request.Headers.TryGetValue(Headers.Trace, out var traceHeaderValue) && !string.IsNullOrWhiteSpace(traceHeaderValue)
            ? traceHeaderValue.ToString()
            : context.TraceIdentifier;

        context.Response.Headers.TryAdd(Headers.Trace, traceId);
        
        using var logcontext = LogContext.PushProperty("TraceIdentifier", traceId);

        await next(context);
    }
}
