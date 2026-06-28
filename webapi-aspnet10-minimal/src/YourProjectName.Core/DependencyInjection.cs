using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Core.Abstractions.Decorators;

namespace YourProjectName.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        //Register application services here
        var assembly = typeof(DependencyInjection).Assembly;

        //Register handlers and validators
        services
            .AddHandlers()
            .AddValidatorsFromAssembly(assembly)
            .AddDecorators();

        return services;
    }

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
        //This decorator will create an activity span for each handler
        services.AddHandlerDecorator(typeof(ICommandHandler<,>), typeof(TracingDecorator.CommandHandler<,>));
        services.AddHandlerDecorator(typeof(ICommandHandler<>), typeof(TracingDecorator.CommandBaseHandler<>));
        services.AddHandlerDecorator(typeof(IQueryHandler<,>), typeof(TracingDecorator.QueryHandler<,>));

        return services;
    }
}
