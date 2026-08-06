using Carter;

namespace YourProjectName.WebApi.DependencyInjectionExtensions;

internal static class AddEndpointsExtension
{
    //Discover all Carter endpoint modules in the executing assembly
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.AddCarter();

        return services;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapCarter();
    }
}
