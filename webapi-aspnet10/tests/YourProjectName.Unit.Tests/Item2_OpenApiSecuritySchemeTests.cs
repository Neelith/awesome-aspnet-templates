namespace YourProjectName.Unit.Tests;

/// <summary>
/// Item 2 — OpenAPI JWT SecurityScheme via AddDocumentTransformer.
/// Current: bare AddOpenApi() with no security scheme.
/// Target: AddDocumentTransformer with Bearer JWT SecuritySchemeType.Http.
/// </summary>
public sealed class Item2_OpenApiSecuritySchemeTests
{
    private static readonly string RepoRootPath = RepoRoot.Find();

    private static readonly string OpenApiPath = Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
        "YourProjectName.WebApi", "Infrastructure", "Setup", "Extensions", "AddOpenApiExtension.cs");

    private string Content => File.ReadAllText(OpenApiPath);

    [Fact]
    public void FileContains_AddDocumentTransformer()
    {
        var content = Content;
        Assert.True(content.Contains("AddDocumentTransformer", StringComparison.Ordinal),
            "Expected AddOpenApiExtension to use AddDocumentTransformer for OpenAPI security scheme.");
    }

    [Fact]
    public void FileContains_SecuritySchemeTypeHttp()
    {
        var content = Content;
        Assert.True(content.Contains("SecuritySchemeType.Http", StringComparison.Ordinal),
            "Expected AddOpenApiExtension to define SecuritySchemeType.Http.");
    }

    [Fact]
    public void FileContains_BearerFormatJwt()
    {
        var content = Content;
        Assert.True(content.Contains("BearerFormat = \"JWT\"", StringComparison.Ordinal),
            "Expected AddOpenApiExtension to set BearerFormat = \"JWT\".");
    }

    [Fact]
    public void FileContains_OpenApiSecuritySchemeReference()
    {
        var content = Content;
        Assert.True(content.Contains("OpenApiSecuritySchemeReference(\"Bearer\")", StringComparison.Ordinal),
            "Expected AddOpenApiExtension to reference Bearer security scheme via " +
            "OpenApiSecuritySchemeReference(\"Bearer\").");
    }

    [Fact]
    public void FileContains_OpenApiSecurityRequirement()
    {
        var content = Content;
        Assert.True(content.Contains("OpenApiSecurityRequirement", StringComparison.Ordinal),
            "Expected AddOpenApiExtension to add an OpenApiSecurityRequirement.");
    }
}
