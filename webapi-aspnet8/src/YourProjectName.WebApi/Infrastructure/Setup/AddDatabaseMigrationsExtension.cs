using Microsoft.EntityFrameworkCore;
using YourProjectName.Infrastructure.Persistence;

namespace YourProjectName.WebApi.Infrastructure.Setup;

internal static class AddDatabaseMigrationsExtension
{
    public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dbContext is not null &&
            dbContext.Database.CanConnect())
        {
            dbContext.Database.Migrate();
        }

    }
}
