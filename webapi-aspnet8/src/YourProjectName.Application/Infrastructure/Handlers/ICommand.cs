using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Infrastructure.Handlers;
public interface ICommand : ICommand<Result>;
public interface ICommand<TResponse>;
