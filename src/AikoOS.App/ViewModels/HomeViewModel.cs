using System;
using System.Windows.Threading;
using AikoOS.Runtime.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AikoOS.App.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IRuntimeState _runtimeState;
    private readonly DispatcherTimer _uptimeTimer;

    public HomeViewModel(
        IRuntimeState runtimeState,
        UnityControlViewModel unityControl)
    {
        _runtimeState = runtimeState;
        UnityControl = unityControl;

        _runtimeState.StateChanged +=
            OnRuntimeStateChanged;

        RuntimeStatusText =
            _runtimeState.Status.ToString();

        StartedAtText =
            _runtimeState.StartedAt
                .ToLocalTime()
                .ToString("yyyy-MM-dd HH:mm:ss");

        UpdateUptime();

        _uptimeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _uptimeTimer.Tick +=
            OnUptimeTimerTick;

        _uptimeTimer.Start();
    }

    public UnityControlViewModel UnityControl
    {
        get;
    }

    [ObservableProperty]
    private string _welcomeMessage =
        "Welcome to AikoOS.";

    [ObservableProperty]
    private int _clickCount;

    [ObservableProperty]
    private string _runtimeStatusText =
        "Starting";

    [ObservableProperty]
    private string _startedAtText =
        string.Empty;

    [ObservableProperty]
    private string _uptimeText =
        "00:00:00";

    [RelayCommand]
    private void IncreaseCount()
    {
        ClickCount++;

        WelcomeMessage =
            $"Button clicked {ClickCount} time(s).";
    }

    private void OnRuntimeStateChanged(
        object? sender,
        EventArgs e)
    {
        RuntimeStatusText =
            _runtimeState.Status.ToString();
    }

    private void OnUptimeTimerTick(
        object? sender,
        EventArgs e)
    {
        UpdateUptime();
    }

    private void UpdateUptime()
    {
        TimeSpan uptime =
            _runtimeState.Uptime;

        UptimeText =
            $"{(int)uptime.TotalHours:00}:" +
            $"{uptime.Minutes:00}:" +
            $"{uptime.Seconds:00}";
    }
}