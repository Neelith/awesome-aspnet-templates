using Microsoft.Extensions.Logging;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Shared.Results;

namespace YourProjectName.Application.Infrastructure.Decorators;
internal static class LoggingDecorator
{
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing command {CommandName}", typeof(TCommand).Name);

            Result result = await inner.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {CommandName}", typeof(TCommand).Name);
            }
            else
            {
                var errors = result.Error is ValidationError validationError
                  ? string.Join(", ", validationError.Errors.Select(e => e.Description))
                  : result.Error.Description;

                logger.LogError("Completed command {CommandName} with one or more errors. Error code: {ErrorCode}. Error type: {ErrorType}. Errors: {Errors}",
                    typeof(TCommand).Name,
                    result.Error.Code,
                    result.Error.Type,
                    errors);
            }

            return result;
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing command {CommandName}", typeof(TCommand).Name);

            Result<TResponse> result = await inner.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {CommandName}", typeof(TCommand).Name);
            }
            else
            {
                var errors = result.Error is ValidationError validationError
                   ? string.Join(", ", validationError.Errors.Select(e => e.Description))
                   : result.Error.Description;

                logger.LogError("Completed command {CommandName} with one or more errors. Error code: {ErrorCode}. Error type: {ErrorType}. Errors: {Errors}",
                    typeof(TCommand).Name,
                    result.Error.Code,
                    result.Error.Type,
                    errors);
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing query {Query}", query);

            Result<TResponse> result = await inner.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed query {QueryName}", typeof(TQuery).Name);
            }
            else
            {
                var errors = result.Error is ValidationError validationError
                    ? string.Join(", ", validationError.Errors.Select(e => e.Description))
                    : result.Error.Description;

                logger.LogError("Completed query {QueryName} with one or more errors. Error code: {ErrorCode}. Error type: {ErrorType}. Errors: {Errors}",
                    typeof(TQuery).Name,
                    result.Error.Code,
                    result.Error.Type,
                    errors);
            }

            return result;
        }
    }
}
