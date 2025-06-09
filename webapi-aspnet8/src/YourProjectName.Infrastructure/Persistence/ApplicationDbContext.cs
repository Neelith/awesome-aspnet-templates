using System.Reflection;
using Microsoft.EntityFrameworkCore;
using YourProjectName.Application.Infrastructure.Persistance;
using YourProjectName.Domain.WeatherForecasts;
using YourProjectName.Shared.Domain;
using YourProjectName.Shared.Time;

namespace YourProjectName.Infrastructure.Persistence
{
    internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeProvider dateTimeProvider) : DbContext(options), IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetUpdatedAtUtcOnEveryAuditableEntity();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetUpdatedAtUtcOnEveryAuditableEntity()
        {
            var entitiesBeignUpdated = ChangeTracker.Entries<AuditableEntity>()
                .Where(entry => entry.State == EntityState.Modified);

            foreach (var entry in entitiesBeignUpdated)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = dateTimeProvider.UtcNow;
                }
            }
        }

        public DbSet<WeatherForecast> Forecasts { get; set; }
    }
}
