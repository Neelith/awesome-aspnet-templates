using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using YourProjectName.Infrastructure;
using YourProjectName.WebApi.Constants;

namespace YourProjectName.WebApi.DependencyInjectionExtensions;

internal static class AddHealthCheckExtension
{
    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", _ => HealthCheckResult.Healthy(), new[] { Tags.Liveness });

        return services;
    }

    public static void UseAppHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(Tags.Liveness)
        }).AllowAnonymous();

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(Tags.Readiness),
            ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
        }).AllowAnonymous();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
        }).AllowAnonymous();
    }
}

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
