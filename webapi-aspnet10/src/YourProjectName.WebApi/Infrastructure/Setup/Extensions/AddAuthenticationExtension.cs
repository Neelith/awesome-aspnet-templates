using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using YourProjectName.WebApi.Infrastructure.Settings;

namespace YourProjectName.WebApi.Infrastructure.Setup.Extensions;

public static class AddAuthenticationExtension
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        JwtSettings jwtSettings,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(jwtSettings);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            //Https metadata is only disabled for the local development environment
            options.RequireHttpsMetadata = !environment.IsEnvironment("Local");
            options.Authority = jwtSettings.Authority;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };
        });

        return services;
    }
}
