using System.Windows;
using System.IO;
using AikoOS.App.Extensions;
using AikoOS.App.Services;
using AikoOS.App.ViewModels;
using AikoOS.Behavior;
using AikoOS.Live2D.Services;
using AikoOS.App.Avatar;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace AikoOS.App;

public partial class App : Application
{
    private IHost? _host;

    private void OnDispatcherUnhandledException(
    object sender,
    System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            e.Exception.ToString(),
            "AikoOS crashed",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {

        DispatcherUnhandledException += OnDispatcherUnhandledException;
        base.OnStartup(e);

        try
        {
            string envPath = Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "..",
                    ".env"));

            if (!File.Exists(envPath))
            {
                throw new FileNotFoundException(
                    $"Không tìm thấy file .env tại:\n{envPath}");
            }

            Env.Load(envPath);

            var settings = new HostApplicationBuilderSettings
            {
                ContentRootPath = AppContext.BaseDirectory
            };

            var builder = Host.CreateApplicationBuilder(settings);

            builder.Configuration
                .AddJsonFile(
                    "config/config.json",
                    optional: false,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.AddAikoOSLogging();

            builder.Services.AddSingleton<
    AikoOS.Core.Services.IAikoStateService,
    AikoOS.Behavior.Services.AikoStateService>();

            builder.Services.AddAikoOSApplication(
                builder.Configuration);

            _host = builder.Build();

            await _host.StartAsync();

            var mainWindow =
                _host.Services.GetRequiredService<MainWindow>();

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            mainWindow.Closed += MainWindow_Closed;

            MainWindow = mainWindow;

            mainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.ToString(),
                "AikoOS Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(-1);
        }
    }

    private void MainWindow_Closed(
        object? sender,
        EventArgs e)
    {
        Shutdown();
    }

    protected override async void OnExit(
        ExitEventArgs e)
    {
        if (_host is not null)
        {
            try
            {
                ILogger<App> logger =
                    _host.Services.GetRequiredService<
                        ILogger<App>>();

                logger.LogInformation(
                    "AikoOS is shutting down.");

                await _host.StopAsync(
                    TimeSpan.FromSeconds(5));
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.ToString(),
                    "AikoOS Shutdown Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _host.Dispose();
            }
        }

        base.OnExit(e);
    }
}