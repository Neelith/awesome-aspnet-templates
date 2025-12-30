using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Application.Infrastructure.User;

namespace YourProjectName.Infrastructure.User;

internal static class AddCurrentUserServiceExtensions
{
    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
