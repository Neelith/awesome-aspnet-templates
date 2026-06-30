using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using YourProjectName.Core.Abstractions.Diagnostics;
using YourProjectName.WebApi.Settings;

namespace YourProjectName.WebApi.DependencyInjectionExtensions;

internal static class AddTelemetryExtension
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, OpenTelemetrySettings telemetrySettings, IHostEnvironment environment, bool enableRedis)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(telemetrySettings.ServiceName, telemetrySettings.ServiceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(ApplicationDiagnostics.ActivitySourceName);

                if (enableRedis)
                {
                    tracing.AddRedisInstrumentation();
                }

                if (environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }

                if (!string.IsNullOrEmpty(telemetrySettings.OtlpEndpoint))
                {
                    tracing.AddOtlpExporter(options => options.Endpoint = new Uri(telemetrySettings.OtlpEndpoint));
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsqlInstrumentation()
                    .AddRuntimeInstrumentation();

                if (!environment.IsEnvironment("Local"))
                {
                    metrics.AddConsoleExporter();
                }

                if (!string.IsNullOrEmpty(telemetrySettings.OtlpEndpoint))
                {
                    metrics.AddOtlpExporter(options => options.Endpoint = new Uri(telemetrySettings.OtlpEndpoint));
                }

                if (!string.IsNullOrEmpty(telemetrySettings.MetricsOtlpEndpoint))
                {
                    metrics.AddOtlpExporter(options => options.Endpoint = new Uri(telemetrySettings.MetricsOtlpEndpoint));
                }
            });

        return services;
    }
}
