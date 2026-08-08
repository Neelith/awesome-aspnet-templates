using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using YourProjectName.Shared.Constants;

namespace YourProjectName.WebApi.Infrastructure.Setup.Extensions;

internal static class AddHealthCheckExtension
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", _ => HealthCheckResult.Healthy(), [HealthCheckTags.Liveness]);

        return services;
    }

    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
        //Liveness carries no details and is meant for orchestrator probes
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(HealthCheckTags.Liveness)
        }).AllowAnonymous();

        //Readiness and full health reports expose infrastructure details, so they require authentication
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(HealthCheckTags.Readiness),
            ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
        }).RequireAuthorization();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteResponseAsync
        }).RequireAuthorization();
    }
}
