using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YourProjectName.Infrastructure.Persistence;

public static class AddDatabaseMigrationsExtension
{
    public static void ApplyDatabaseMigrations(IServiceScope scope, ILogger logger)
    {
        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}
