using System.Diagnostics;

namespace YourProjectName.Shared.Diagnostics;

public static class ApplicationDiagnostics
{
    public const string ActivitySourceName = "YourProjectName.Application";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
