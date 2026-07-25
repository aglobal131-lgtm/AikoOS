using System.Windows;
using AikoOS.App.Extensions;
using AikoOS.App.Services;
using AikoOS.App.ViewModels;
using AikoOS.Behavior;
using AikoOS.Infrastructure.Communication;
using AikoOS.Live2D.Services;
using AikoOS.App.Avatar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AikoOS.App;

public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(
        StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            HostApplicationBuilderSettings settings = new()
            {
                ContentRootPath = AppContext.BaseDirectory
            };

            HostApplicationBuilder builder =
                Host.CreateApplicationBuilder(settings);

            builder.Configuration.AddJsonFile(
                path: "config/config.json",
                optional: false,
                reloadOnChange: true);

            builder.AddAikoOSLogging();

            builder.Services.AddAikoOSApplication(
                builder.Configuration);

            builder.Services.AddBehavior(
                builder.Configuration);

            // Unity communication
            builder.Services.AddSingleton<
                IUnityTransport,
                NamedPipeUnityTransport>();

            builder.Services.AddSingleton<
                UnityCommandService>();

            builder.Services.AddSingleton<
                UnityControlViewModel>();

            _host = builder.Build();

            _host.Services.GetRequiredService<
                AvatarStateBridge>();

            _host.Services.GetRequiredService<
                BehaviorAvatarSyncService>();

            await _host.StartAsync();

            ILogger<App> logger =
                _host.Services.GetRequiredService<
                    ILogger<App>>();

            logger.LogInformation(
                "AikoOS is starting in {EnvironmentName}.",
                builder.Environment.EnvironmentName);

            MainWindow mainWindow =
                _host.Services.GetRequiredService<
                    MainWindow>();

            MainWindow = mainWindow;

            mainWindow.Closed +=
                MainWindow_Closed;

            mainWindow.Show();
            mainWindow.Activate();

            logger.LogInformation(
    "AikoOS MainWindow was displayed successfully.");
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