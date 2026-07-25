using AikoOS.Core.Models;
using AikoOS.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AikoOS.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IUserSettingsService _settingsService;
    private readonly ILogger<SettingsViewModel> _logger;

    public SettingsViewModel(
        IUserSettingsService settingsService,
        ILogger<SettingsViewModel> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [ObservableProperty]
    private string _assistantName = "Aiko";

    [ObservableProperty]
    private bool _startWithWindows;

    [ObservableProperty]
    private bool _minimizeToTray = true;

    [ObservableProperty]
    private string _statusMessage = "Press Load settings.";

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Loading settings...";

            UserSettings settings =
                await _settingsService.LoadAsync();

            AssistantName = settings.AssistantName;
            StartWithWindows = settings.StartWithWindows;
            MinimizeToTray = settings.MinimizeToTray;

            StatusMessage = "Settings loaded.";
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Could not load user settings.");

            StatusMessage = "Could not load settings.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
        {
            return;
        }

        string name = AssistantName.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage =
                "Assistant name cannot be empty.";

            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Saving settings...";

            UserSettings settings = new()
            {
                AssistantName = name,
                StartWithWindows = StartWithWindows,
                MinimizeToTray = MinimizeToTray
            };

            await _settingsService.SaveAsync(settings);

            AssistantName = name;
            StatusMessage = $"Settings saved for {name}.";
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Could not save user settings.");

            StatusMessage = "Could not save settings.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}