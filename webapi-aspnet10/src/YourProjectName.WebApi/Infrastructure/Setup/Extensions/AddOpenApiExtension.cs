using Microsoft.OpenApi;
using YourProjectName.WebApi.Infrastructure.Settings;

namespace YourProjectName.WebApi.Infrastructure.Setup.Extensions;

internal static class AddOpenApiExtension
{
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddEndpointsApiExplorer();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
#pragma warning disable CS8602
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = jwtSettings.Authority is not null
                        ? $"JWT Authorization header using the Bearer scheme. Authority: {jwtSettings.Authority}"
                        : "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header
                };
#pragma warning restore CS8602

                document.Security ??= [];
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer"),
                        []
                    }
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static void UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.RoutePrefix = "openapi";
            });
        }
    }
}
