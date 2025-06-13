using Microsoft.Extensions.DependencyInjection;
using YourProjectName.Domain.WeatherForecasts.Repositories.WeatherForecastRepository;

namespace YourProjectName.Infrastructure.Persistence.Repositories;
internal static class AddRepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

        return services;
    }
}
