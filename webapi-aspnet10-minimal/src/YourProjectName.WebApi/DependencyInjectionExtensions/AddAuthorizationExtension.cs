namespace YourProjectName.WebApi.DependencyInjectionExtensions;

public static class AddAuthorizationExtension
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }
}
