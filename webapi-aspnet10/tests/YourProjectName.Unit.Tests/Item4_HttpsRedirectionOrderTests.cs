namespace YourProjectName.Unit.Tests;

/// <summary>
/// Item 4 — HttpsRedirection moved after UseOpenApi, before migrations.
/// Current order: UseHttpsRedirection BEFORE UseOpenApi and BEFORE UseAuthentication.
/// Target: UseHttpsRedirection AFTER UseOpenApi and AFTER UseAuthentication.
/// </summary>
public sealed class Item4_HttpsRedirectionOrderTests
{
    private static readonly string RepoRootPath = RepoRoot.Find();

    private static readonly string DiPath = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
        "YourProjectName.WebApi", "Infrastructure", "Setup", "DependencyInjection.cs");

    private string Content => File.ReadAllText(DiPath);

    [Fact]
    public void UseHttpsRedirection_After_UseOpenApi()
    {
        var content = Content;
        var httpsIdx = content.IndexOf("UseHttpsRedirection", StringComparison.Ordinal);
        var openApiIdx = content.IndexOf("UseOpenApi", StringComparison.Ordinal);

        Assert.True(httpsIdx > openApiIdx,
            $"Expected UseHttpsRedirection (index {httpsIdx}) to come AFTER UseOpenApi (index {openApiIdx}) " +
            "in the pipeline. HttpsRedirection must be placed after OpenAPI documentation middleware.");
    }

    [Fact]
    public void UseHttpsRedirection_After_UseAuthentication()
    {
        var content = Content;
        var httpsIdx = content.IndexOf("UseHttpsRedirection", StringComparison.Ordinal);
        var authIdx = content.IndexOf("UseAuthentication", StringComparison.Ordinal);

        Assert.True(httpsIdx > authIdx,
            $"Expected UseHttpsRedirection (index {httpsIdx}) to come AFTER UseAuthentication (index {authIdx}) " +
            "in the pipeline. HttpsRedirection must be placed after auth middleware.");
    }
}
