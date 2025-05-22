namespace YourProjectName.WebApi.Infrastructure.Setup;

internal static class AddSettingsExtension
{
    public static TSettings? AddSettings<TSettings>(this IServiceCollection services, IConfiguration configuration) where TSettings : class, new()
    {
        var settingsSection = configuration.GetSection(typeof(TSettings).Name);

        if (settingsSection is null)
        {
            return null;
        }

        services.Configure<TSettings>(settingsSection);

        return settingsSection.Get<TSettings>();
    }
}
