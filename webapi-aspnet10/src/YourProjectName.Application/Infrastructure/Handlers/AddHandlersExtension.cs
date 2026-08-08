using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Application.Infrastructure.Decorators;

namespace YourProjectName.Application.Infrastructure.Handlers;

public static class AddHandlersExtension
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        //Register the query handlers
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddHandlers([assembly]);

        return services;
    }

    public static IServiceCollection AddDecorators(this IServiceCollection services)
    {
        //Here we decorate the command handlers with the validation decorator
        //This decorator will validate the command before executing it
        services.AddHandlerDecorator(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.AddHandlerDecorator(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));
        services.AddHandlerDecorator(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));

        //Here we decorate the handlers with the tracing decorator
        //This decorator will start an OTel Activity span per handler
        services.AddHandlerDecorator(typeof(ICommandHandler<,>), typeof(TracingDecorator.CommandHandler<,>));
        services.AddHandlerDecorator(typeof(ICommandHandler<>), typeof(TracingDecorator.CommandBaseHandler<>));
        services.AddHandlerDecorator(typeof(IQueryHandler<,>), typeof(TracingDecorator.QueryHandler<,>));

        return services;
    }
}