namespace YourProjectName.Unit.Tests;

/// <summary>
/// Fix B — Migrations must always run (no environment gate).
/// RED: DependencyInjection.cs contains IsDevelopment()/IsEnvironment("Local") guard.
/// GREEN: guard removed, ApplyDatabaseMigrations called unconditionally.
/// </summary>
public sealed class FixB_MigrationGateRemovalTests
{
    private static readonly string RepoRootPath = RepoRoot.Find();

    private static string MigrationFileContent => File.ReadAllText(
        Path.Combine(RepoRootPath, "webapi-aspnet10", "src",
            "YourProjectName.WebApi", "Infrastructure", "Setup", "DependencyInjection.cs"));

    /// <summary>
    /// The migrations region must NOT contain an environment gate.
    /// Current state: if (IsDevelopment() || IsEnvironment("Local")) → test fails RED.
    /// </summary>
    [Fact]
    public void DependencyInjection_MigrationsRegion_NoEnvironmentGate()
    {
        var content = MigrationFileContent;

        var hasIsDevelopment = content.Contains("IsDevelopment()", StringComparison.Ordinal);
        var hasIsEnvironment = content.Contains("IsEnvironment(\"Local\")", StringComparison.Ordinal);

        Assert.False(hasIsDevelopment || hasIsEnvironment,
            "Expected migrations region to have NO environment gate (IsDevelopment/IsEnvironment), " +
            "but at least one was found. Migrations must run unconditionally.");
    }

    /// <summary>
    /// ApplyDatabaseMigrations must still be called (unconditionally).
    /// Current state: it is called inside the guarded block → this assertion passes.
    /// </summary>
    [Fact]
    public void DependencyInjection_MigrationsRegion_CallsApplyDatabaseMigrations()
    {
        var content = MigrationFileContent;

        var callsApplyMigrations = content.Contains("ApplyDatabaseMigrations", StringComparison.Ordinal);

        Assert.True(callsApplyMigrations,
            "Expected DependencyInjection.cs to still call ApplyDatabaseMigrations.");
    }
}
