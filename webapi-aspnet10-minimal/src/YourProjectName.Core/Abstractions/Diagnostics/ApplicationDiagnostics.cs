using System.Diagnostics;

namespace YourProjectName.Core.Abstractions.Diagnostics;

public static class ApplicationDiagnostics
{
    public const string ActivitySourceName = "YourProjectName.Core";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
