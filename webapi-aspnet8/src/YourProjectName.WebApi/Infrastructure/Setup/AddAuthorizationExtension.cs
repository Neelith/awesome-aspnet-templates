using Microsoft.AspNetCore.Authorization;

namespace YourProjectName.WebApi.Infrastructure.Setup;

public static class AddAuthorizationExtension
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }
}
