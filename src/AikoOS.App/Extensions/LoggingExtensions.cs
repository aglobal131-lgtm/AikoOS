using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace AikoOS.App.Extensions;

public static class LoggingExtensions
{
    public static HostApplicationBuilder AddAikoOSLogging(
        this HostApplicationBuilder builder)
    {
        string logDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "AikoOS",
            "logs");

        Directory.CreateDirectory(logDirectory);

        string logFilePath = Path.Combine(
            logDirectory,
            "aikoos-.log");

        builder.Logging.ClearProviders();

        builder.Services.AddSerilog(
            loggerConfiguration =>
            {
                loggerConfiguration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override(
                        "Microsoft",
                        LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Debug()
                    .WriteTo.File(
                        path: logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        shared: true,
                        outputTemplate:
                            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] " +
                            "[{Level:u3}] " +
                            "{SourceContext}: " +
                            "{Message:lj}" +
                            "{NewLine}{Exception}");
            });

        return builder;
    }
}