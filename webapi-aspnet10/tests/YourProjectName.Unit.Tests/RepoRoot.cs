namespace YourProjectName.Unit.Tests;

internal static class RepoRoot
{
    /// <summary>
    /// Finds the repository root by walking up the directory tree from the test assembly's
    /// location and looking for the marker path.
    /// </summary>
    public static string Find()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var marker = Path.Combine(dir.FullName, "webapi-aspnet10", "src",
                "YourProjectName.WebApi", "Infrastructure", "Setup", "DependencyInjection.cs");
            if (File.Exists(marker))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find repository root. Marker not found.");
    }
}
