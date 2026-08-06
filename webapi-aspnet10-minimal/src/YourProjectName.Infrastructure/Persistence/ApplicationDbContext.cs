using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Core.Abstractions.Persistence;
using YourProjectName.Core.Entities.WeatherForecasts;
using YourProjectName.Core.Services.Time;
using YourProjectName.Core.Services.User;
using YourProjectName.Core.Shared;

namespace YourProjectName.Infrastructure.Persistence;

internal class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDateTimeProvider dateTimeProvider,
    ICurrentUserService currentUserService,
    IServiceProvider serviceProvider)
    : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SetAuditablePropertiesOnCreatedEntities();

        SetAuditablePropertiesOnUpdatedEntities();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    private void SetAuditablePropertiesOnCreatedEntities()
    {
        var entitiesBeingCreated = ChangeTracker.Entries<AuditableEntity>()
            .Where(entry => entry.State == EntityState.Added);

        string createdBy = currentUserService.IsCurrentUserAuthenticated()
            ? currentUserService.GetCurrentUserId()
            : "system";

        foreach (var entry in entitiesBeingCreated)
        {
            entry.Entity.CreatedAtUtc = dateTimeProvider.UtcNow;
            entry.Entity.CreatedBy = createdBy;
        }
    }

    private void SetAuditablePropertiesOnUpdatedEntities()
    {
        var entitiesBeingUpdated = ChangeTracker.Entries<AuditableEntity>()
            .Where(entry => entry.State == EntityState.Modified);

        string updatedBy = currentUserService.IsCurrentUserAuthenticated()
            ? currentUserService.GetCurrentUserId()
            : "system";

        foreach (var entry in entitiesBeingUpdated)
        {
            entry.Entity.UpdatedAtUtc = dateTimeProvider.UtcNow;
            entry.Entity.UpdatedBy = updatedBy;
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var entitiesWithEvents = ChangeTracker.Entries<Entity>()
            .Where(entry => entry.Entity.DomainEvents.Count > 0)
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entitiesWithEvents.SelectMany(entity => entity.DomainEvents).ToList();

        entitiesWithEvents.ForEach(entity => entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

            foreach (var handler in serviceProvider.GetServices(handlerType))
            {
                if (handler is null)
                {
                    continue;
                }

                if (handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle))?.Invoke(handler, [domainEvent, cancellationToken]) is Task task)
                {
                    await task;
                }
            }
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (Database.CurrentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("No transaction is in progress to commit.");
        }

        await Database.CurrentTransaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        if (Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("No transaction is in progress to roll back.");
        }

        await Database.CurrentTransaction.RollbackAsync(cancellationToken);
    }

    public DbSet<WeatherForecast> Forecasts { get; set; }
}
