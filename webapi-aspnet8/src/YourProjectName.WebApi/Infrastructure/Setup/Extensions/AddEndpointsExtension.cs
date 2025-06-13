using System.Reflection;
using Carter;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YourProjectName.WebApi.Endpoints;

namespace YourProjectName.WebApi.Infrastructure.Setup.Extensions;

internal static class AddEndpointsExtension
{
    //Discover all endpoints that implement the IEndpoints interface
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        services.AddCarter();

        return services;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapCarter();
    }
}
