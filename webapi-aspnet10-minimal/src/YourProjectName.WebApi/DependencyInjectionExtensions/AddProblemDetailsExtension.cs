using System.Diagnostics;

namespace YourProjectName.WebApi.DependencyInjectionExtensions;

internal static class AddProblemDetailsExtension
{
    public static IServiceCollection ConfigureProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
                    options.CustomizeProblemDetails = context =>
                    {
                        var httpContext = context.HttpContext;

                        var instance = httpContext.Request.Path;
                        context.ProblemDetails.Instance = instance;

                        if (context.Exception is null)
                        {
                            var method = httpContext.Request.Method;
                            context.ProblemDetails.Extensions.TryAdd("method", method);

                            context.ProblemDetails.Extensions.TryAdd("endpoint", $"{method} {instance}");
                        }

                        string traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

                        context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
                    });

        return services;
    }
}
