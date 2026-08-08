namespace YourProjectName.Unit.Tests;

/// <summary>
/// Fix C — Remove TraceMiddleware and Headers.cs; use Activity.Current?.TraceId in ProblemDetails.
/// RED: TraceMiddleware.cs exists, Headers.cs exists, UseMiddleware&lt;TraceMiddleware&gt; in DI,
/// AddProblemDetailsExtension uses Headers.Trace instead of Activity.Current?.TraceId.
/// GREEN: files deleted, DI clean, ProblemDetails uses Activity.Current?.TraceId.
/// </summary>
public sealed class FixC_TraceMiddlewareRemovalTests
{
    private static readonly string RepoRootPath = RepoRoot.Find();

    private static readonly string SetupDir = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
        "YourProjectName.WebApi", "Infrastructure", "Setup");

    [Fact]
    public void TraceMiddleware_File_DoesNotExist()
    {
        var filePath = Path.Combine(SetupDir, "Middlewares", "TraceMiddleware.cs");
        Assert.False(File.Exists(filePath),
            $"Expected file NOT to exist: {filePath}");
    }

    [Fact]
    public void Headers_File_DoesNotExist()
    {
        var filePath = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
            "YourProjectName.WebApi", "Constants", "Headers.cs");
        Assert.False(File.Exists(filePath),
            $"Expected file NOT to exist: {filePath}");
    }

    [Fact]
    public void DependencyInjection_DoesNotContain_UseTraceMiddleware()
    {
        var diPath = Path.Combine(SetupDir, "DependencyInjection.cs");
        var content = File.ReadAllText(diPath);

        var contains = content.Contains("UseMiddleware<TraceMiddleware>", StringComparison.Ordinal);
        Assert.False(contains,
            "Expected DependencyInjection.cs NOT to contain UseMiddleware<TraceMiddleware>.");
    }

    [Fact]
    public void AddProblemDetailsExtension_Uses_ActivityCurrentTraceId()
    {
        var path = Path.Combine(SetupDir, "Extensions", "AddProblemDetailsExtension.cs");
        var content = File.ReadAllText(path);

        var contains = content.Contains("Activity.Current?.TraceId", StringComparison.Ordinal);
        Assert.True(contains,
            "Expected AddProblemDetailsExtension.cs to use Activity.Current?.TraceId " +
            "for the traceId, but the expression was not found.");
    }

    [Fact]
    public void AddProblemDetailsExtension_DoesNotContain_HeadersDotTrace()
    {
        var path = Path.Combine(SetupDir, "Extensions", "AddProblemDetailsExtension.cs");
        var content = File.ReadAllText(path);

        var contains = content.Contains("Headers.Trace", StringComparison.Ordinal);
        Assert.False(contains,
            "Expected AddProblemDetailsExtension.cs NOT to reference Headers.Trace.");
    }
}
