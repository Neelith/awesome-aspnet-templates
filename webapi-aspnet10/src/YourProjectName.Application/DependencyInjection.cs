using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Domain.Shared;

namespace YourProjectName.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Register application services here
        var assembly = typeof(DependencyInjection).Assembly;

        //Register handlers and validators
        services
            .AddHandlers()
            .AddValidatorsFromAssembly(assembly)
            .AddDecorators();

        //Register domain event handlers
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
