using Serilog;
using YourProjectName.WebApi.Infrastructure.Middlewares;

namespace YourProjectName.WebApi.Infrastructure.Setup;

public static class AddLoggingExtension
{
    public static void AddLogging(this WebApplicationBuilder webApplicationBuilder)
    {
        //clear the registered logging providers
        webApplicationBuilder.Logging.ClearProviders();

        //configure serilog for logging
        webApplicationBuilder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));
    }

    public static void UseLogging(this WebApplication app)
    {
        app.UseMiddleware<TraceLoggerMiddleware>();

        app.UseSerilogRequestLogging();
    }
}
