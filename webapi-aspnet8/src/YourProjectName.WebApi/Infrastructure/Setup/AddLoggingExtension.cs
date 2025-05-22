using Serilog;
using Serilog.Extensions.Logging;
using YourProjectName.WebApi.Infrastructure.Middlewares;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace YourProjectName.WebApi.Infrastructure.Setup;

public static class AddLoggingExtension
{
    public static ILogger AddLogging(this WebApplicationBuilder webApplicationBuilder)
    {
        //clear the registered logging providers
        webApplicationBuilder.Logging.ClearProviders();

        //configure serilog for logging
        webApplicationBuilder.Host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));

        return CreateStartupLogger(webApplicationBuilder.Configuration);
    }

    public static void UseLogging(this WebApplication app)
    {
        app.UseMiddleware<TraceLoggerMiddleware>();

        app.UseSerilogRequestLogging();
    }

    private static ILogger CreateStartupLogger(IConfiguration configuration)
    {
        var serilogLogger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        return new SerilogLoggerFactory(serilogLogger).CreateLogger("Startup");
    }
}
