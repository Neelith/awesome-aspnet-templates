using System.Diagnostics;
using YourProjectName.Core.Abstractions.Diagnostics;

namespace YourProjectName.Core.Abstractions.Decorators;

internal static class TracingDecorator
{
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner)
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            using Activity? activity = ApplicationDiagnostics.ActivitySource.StartActivity(typeof(TCommand).Name);

            Result result = await inner.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                activity?.SetStatus(ActivityStatusCode.Error, errors);
            }

            return result;
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : IResponse
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            using Activity? activity = ApplicationDiagnostics.ActivitySource.StartActivity(typeof(TCommand).Name);

            Result<TResponse> result = await inner.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                activity?.SetStatus(ActivityStatusCode.Error, errors);
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : IResponse
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            using Activity? activity = ApplicationDiagnostics.ActivitySource.StartActivity(typeof(TQuery).Name);

            Result<TResponse> result = await inner.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Message));
                activity?.SetStatus(ActivityStatusCode.Error, errors);
            }

            return result;
        }
    }
}
