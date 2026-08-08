using System.Diagnostics;
using Serilog.Context;
using YourProjectName.WebApi.Constants;

namespace YourProjectName.WebApi.Infrastructure.Setup.Middlewares;

public class TraceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        //The trace id is always generated server side (W3C Activity or the connection trace identifier):
        //client-provided header values are never trusted, to avoid log forging.
        string traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        context.Response.Headers.TryAdd(Headers.Trace, traceId);

        using var logcontext = LogContext.PushProperty("TraceIdentifier", traceId);

        await next(context);
    }
}
