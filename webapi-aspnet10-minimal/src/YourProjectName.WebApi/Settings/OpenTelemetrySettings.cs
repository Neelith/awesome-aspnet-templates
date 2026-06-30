namespace YourProjectName.WebApi.Settings;

public class OpenTelemetrySettings
{
    public string ServiceName { get; set; } = "YourProjectName.WebApi";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string? OtlpEndpoint { get; set; }
    public string? MetricsOtlpEndpoint { get; set; }
}
