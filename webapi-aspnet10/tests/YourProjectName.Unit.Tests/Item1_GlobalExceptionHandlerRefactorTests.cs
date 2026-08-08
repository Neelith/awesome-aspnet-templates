namespace YourProjectName.Unit.Tests;

/// <summary>
/// Item 1 — GlobalExceptionHandler refactored to IProblemDetailsService.
/// Current: ctor(ILogger), manually builds ProblemDetails, Activity.Current?.Id, WriteAsJsonAsync.
/// Target: ctor(ILogger, IProblemDetailsService), delegates to TryWriteAsync, no Activity import.
/// </summary>
public sealed class Item1_GlobalExceptionHandlerRefactorTests
{
    private static readonly string RepoRootPath = RepoRoot.Find();

    private static readonly string HandlerPath = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
        "YourProjectName.WebApi", "Infrastructure", "Setup", "Middlewares", "GlobalExceptionHandler.cs");

    private string HandlerContent => File.ReadAllText(HandlerPath);

    [Fact]
    public void HandlerFile_Contains_IProblemDetailsService_Parameter()
    {
        var content = HandlerContent;
        var hasParam = content.Contains("IProblemDetailsService", StringComparison.Ordinal);
        Assert.True(hasParam,
            "Expected GlobalExceptionHandler constructor to take IProblemDetailsService parameter.");
    }

    [Fact]
    public void HandlerFile_Contains_TryWriteAsync()
    {
        var content = HandlerContent;
        var hasCall = content.Contains("TryWriteAsync", StringComparison.Ordinal);
        Assert.True(hasCall,
            "Expected GlobalExceptionHandler to call TryWriteAsync on IProblemDetailsService.");
    }

    [Fact]
    public void HandlerFile_DoesNotContain_WriteAsJsonAsync()
    {
        var content = HandlerContent;
        var has = content.Contains("WriteAsJsonAsync", StringComparison.Ordinal);
        Assert.False(has,
            "Expected GlobalExceptionHandler NOT to contain WriteAsJsonAsync " +
            "(should delegate to IProblemDetailsService.TryWriteAsync).");
    }

    [Fact]
    public void HandlerFile_DoesNotContain_ActivityCurrent()
    {
        var content = HandlerContent;
        var has = content.Contains("Activity.Current", StringComparison.Ordinal);
        Assert.False(has,
            "Expected GlobalExceptionHandler NOT to use Activity.Current (traceId injected by " +
            "ProblemDetails customization elsewhere).");
    }

    [Fact]
    public void HandlerFile_DoesNotContain_SystemDiagnostics()
    {
        var content = HandlerContent;
        var has = content.Contains("using System.Diagnostics;", StringComparison.Ordinal);
        Assert.False(has,
            "Expected GlobalExceptionHandler NOT to import System.Diagnostics.");
    }

    [Fact]
    public void HandlerFile_Contains_LoggerLogError()
    {
        var content = HandlerContent;
        var has = content.Contains("logger.LogError", StringComparison.Ordinal);
        Assert.True(has,
            "Expected GlobalExceptionHandler to still call logger.LogError.");
    }
}
