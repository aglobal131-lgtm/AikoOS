using System;
using System.Threading.Tasks;
using AikoOS.Infrastructure.Communication;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AikoOS.App.ViewModels
{
    public partial class UnityControlViewModel : ObservableObject
    {
        private readonly UnityCommandService _unityCommandService;

        public UnityControlViewModel(
            UnityCommandService unityCommandService)
        {
            _unityCommandService = unityCommandService;

            UpdateConnectionStatus();
        }

        [ObservableProperty]
        private string _connectionStatusText =
            "Disconnected";

        [ObservableProperty]
        private string _lastCommandText =
            "No command sent.";

        [ObservableProperty]
        private bool _isBusy;

        [RelayCommand]
        private async Task ConnectAsync()
        {
            LastCommandText =
                "Connecting to Unity...";

            await ExecuteAsync(
                async () =>
                {
                    bool connected =
                        await _unityCommandService.ConnectAsync();

                    if (!connected)
                    {
                        ConnectionStatusText =
                            "Unable to connect";

                        LastCommandText =
                            "Could not connect. Start Unity Play Mode first.";

                        return;
                    }

                    ConnectionStatusText =
                        "Connected";

                    LastCommandText =
                        "Connected to Unity.";
                });
        }

        [RelayCommand]
        private async Task DisconnectAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    await _unityCommandService.DisconnectAsync();

                    ConnectionStatusText =
                        "Disconnected";

                    LastCommandText =
                        "Disconnected from Unity.";
                });
        }

        [RelayCommand]
        private async Task WalkLeftAsync()
        {
            await SendCommandAsync(
                "Walk Left",
                () => _unityCommandService.WalkLeftAsync());
        }

        [RelayCommand]
        private async Task StopAsync()
        {
            await SendCommandAsync(
                "Stop",
                () => _unityCommandService.StopAsync());
        }

        [RelayCommand]
        private async Task WalkRightAsync()
        {
            await SendCommandAsync(
                "Walk Right",
                () => _unityCommandService.WalkRightAsync());
        }

        private async Task SendCommandAsync(
            string commandName,
            Func<Task> sendAction)
        {
            await ExecuteAsync(
                async () =>
                {
                    await sendAction();

                    UpdateConnectionStatus();

                    LastCommandText =
                        $"Command sent: {commandName}";
                });
        }

        private async Task ExecuteAsync(
            Func<Task> action)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                await action();
            }
            catch (InvalidOperationException exception)
            {
                UpdateConnectionStatus();

                LastCommandText =
                    exception.Message;
            }
            catch (Exception exception)
            {
                UpdateConnectionStatus();

                LastCommandText =
                    $"Unity communication error: {exception.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateConnectionStatus()
        {
            ConnectionStatusText =
                _unityCommandService.IsConnected
                    ? "Connected"
                    : "Disconnected";
        }
    }
}