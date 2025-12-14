using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Application.Infrastructure.Decorators;
using YourProjectName.Application.Infrastructure.Handlers;
using YourProjectName.Shared.Domain;

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

        return services;
    }
}
