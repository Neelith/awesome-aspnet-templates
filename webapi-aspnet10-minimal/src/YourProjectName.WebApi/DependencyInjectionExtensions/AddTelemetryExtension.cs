using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using YourProjectName.Core.Abstractions.Diagnostics;

namespace YourProjectName.WebApi.DependencyInjectionExtensions;

internal static class AddTelemetryExtension
{
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder webApplicationBuilder, bool enableRedis)
    {
        string serviceName = "YourProjectName.WebApi";
        string serviceVersion = "1.0.0";

        webApplicationBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName, serviceVersion))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddSource(ApplicationDiagnostics.ActivitySourceName);

                if (enableRedis)
                {
                    tracing.AddRedisInstrumentation();
                }

                if (webApplicationBuilder.Environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }

                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")))
                {
                    tracing.AddOtlpExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsqlInstrumentation()
                    .AddRuntimeInstrumentation();

                if (webApplicationBuilder.Environment.IsDevelopment())
                {
                    metrics.AddConsoleExporter();
                }

                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")))
                {
                    metrics.AddOtlpExporter();
                }
            });

        return webApplicationBuilder;
    }
}
