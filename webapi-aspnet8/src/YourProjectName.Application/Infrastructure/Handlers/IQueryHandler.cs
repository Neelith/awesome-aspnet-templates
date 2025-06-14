using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Infrastructure.Handlers;
public interface IQueryHandler<in TQuery, TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}