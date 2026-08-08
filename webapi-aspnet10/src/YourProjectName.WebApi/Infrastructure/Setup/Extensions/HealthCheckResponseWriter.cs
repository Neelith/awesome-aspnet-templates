using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace YourProjectName.WebApi.Infrastructure.Setup.Extensions;

internal static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static async Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = report.Status == HealthStatus.Unhealthy
            ? StatusCodes.Status503ServiceUnavailable
            : StatusCodes.Status200OK;

        var payload = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            entries = report.Entries.ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    status = kvp.Value.Status.ToString(),
                    duration = kvp.Value.Duration.ToString(),
                    description = kvp.Value.Description,
                    exception = kvp.Value.Exception?.Message,
                    data = kvp.Value.Data
                })
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, payload, Options, context.RequestAborted);
    }
}
