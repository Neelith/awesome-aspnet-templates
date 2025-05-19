namespace YourProjectName.Application.Infrastructure.Persistance;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
