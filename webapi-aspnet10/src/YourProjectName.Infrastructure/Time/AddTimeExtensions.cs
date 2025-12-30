using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Shared.Time;

namespace YourProjectName.Infrastructure.Time;

internal static class AddTimeExtensions
{
    public static IServiceCollection AddTime(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        return services;
    }
}
