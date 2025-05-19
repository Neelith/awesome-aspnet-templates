using FluentValidation;
using FluentValidation.Results;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Shared.Results;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace YourProjectName.Application.Infrastructure.Decorators;

internal static class ValidationDecorator
{
    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken? cancellationToken = default)
        {
            var context = new ValidationContext<TCommand>(command);

            ValidationFailure[] failures = await ValidateAndCollectFailures(context, validators, cancellationToken);

            if (failures.Length != 0)
            {
                return Result.Fail(new ValidationError(failures));
            }

            return await inner.Handle(command, cancellationToken);
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken? cancellationToken = default)
        {
            var context = new ValidationContext<TCommand>(command);

            ValidationFailure[] failures = await ValidateAndCollectFailures(context, validators, cancellationToken);

            if (failures.Length != 0)
            {
                return Result.Fail<TResponse>(new ValidationError(failures));
            }

            return await inner.Handle(command, cancellationToken);
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken? cancellationToken = default)
        {
            var context = new ValidationContext<TQuery>(query);

            ValidationFailure[] failures = await ValidateAndCollectFailures(context, validators, cancellationToken);

            if (failures.Length != 0)
            {
                return Result.Fail<TResponse>(new ValidationError(failures));
            }

            return await inner.Handle(query, cancellationToken);
        }
    }

    private static async Task<ValidationFailure[]> ValidateAndCollectFailures<T>(
        ValidationContext<T> context, 
        IEnumerable<IValidator<T>> validators, 
        CancellationToken? cancellationToken = default)
    {
        ValidationResult[] validationResults = await Task.WhenAll(
                        validators.Select(v => v.ValidateAsync(context, cancellationToken ?? CancellationToken.None)));

        ValidationFailure[] failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToArray();

        return failures;
    }
}
