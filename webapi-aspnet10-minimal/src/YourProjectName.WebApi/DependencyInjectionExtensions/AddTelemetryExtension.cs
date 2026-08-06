using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using YourProjectName.Core.Abstractions.Diagnostics;
using YourProjectName.WebApi.Settings;

namespace YourProjectName.WebApi.DependencyInjectionExtensions;

internal static class AddTelemetryExtension
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, OpenTelemetrySettings telemetrySettings, IHostEnvironment environment)
    {
        //Console exporters dump telemetry to stdout: useful locally, noise in production,
        //where telemetry should flow through the OTLP exporter instead
        bool useConsoleExporter = environment.IsDevelopment() || environment.IsEnvironment("Local");

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(telemetrySettings.ServiceName, telemetrySettings.ServiceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddSource(ApplicationDiagnostics.ActivitySourceName);

                if (useConsoleExporter)
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

                if (useConsoleExporter)
                {
                    metrics.AddConsoleExporter();
                }

                if (!string.IsNullOrEmpty(telemetrySettings.OtlpEndpoint))
                {
                    metrics.AddOtlpExporter(options => options.Endpoint = new Uri(telemetrySettings.OtlpEndpoint));
                }
            });

        return services;
    }
}
