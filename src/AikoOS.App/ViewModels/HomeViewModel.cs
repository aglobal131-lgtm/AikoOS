using System;
using System.Windows.Threading;
using AikoOS.Runtime.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AikoOS.App.ViewModels;

public partial class HomeViewModel : ObservableObject, IDisposable
{
    private readonly IRuntimeState _runtimeState;
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherTimer _uptimeTimer;

    private bool _isDisposed;

    public HomeViewModel(IRuntimeState runtimeState)
    {
        _runtimeState = runtimeState;
        _dispatcher = Dispatcher.CurrentDispatcher;

        _runtimeState.StateChanged += OnRuntimeStateChanged;

        RuntimeStatusText = _runtimeState.Status.ToString();

        StartedAtText = _runtimeState.StartedAt
            .ToLocalTime()
            .ToString("yyyy-MM-dd HH:mm:ss");

        UpdateUptime();

        _uptimeTimer = new DispatcherTimer(
            DispatcherPriority.Normal,
            _dispatcher)
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _uptimeTimer.Tick += OnUptimeTimerTick;
        _uptimeTimer.Start();
    }

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to AikoOS.";

    [ObservableProperty]
    private int _clickCount;

    [ObservableProperty]
    private string _runtimeStatusText = "Starting";

    [ObservableProperty]
    private string _startedAtText = string.Empty;

    [ObservableProperty]
    private string _uptimeText = "00:00:00";

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
        if (_isDisposed)
        {
            return;
        }

        if (_dispatcher.CheckAccess())
        {
            UpdateRuntimeStatus();
            return;
        }

        _dispatcher.BeginInvoke(
            UpdateRuntimeStatus);
    }

    private void OnUptimeTimerTick(
        object? sender,
        EventArgs e)
    {
        if (_isDisposed)
        {
            return;
        }

        UpdateUptime();
    }

    private void UpdateRuntimeStatus()
    {
        RuntimeStatusText =
            _runtimeState.Status.ToString();
    }

    private void UpdateUptime()
    {
        TimeSpan uptime = _runtimeState.Uptime;

        UptimeText =
            $"{(int)uptime.TotalHours:00}:" +
            $"{uptime.Minutes:00}:" +
            $"{uptime.Seconds:00}";
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        _uptimeTimer.Stop();
        _uptimeTimer.Tick -= OnUptimeTimerTick;

        _runtimeState.StateChanged -=
            OnRuntimeStateChanged;

        GC.SuppressFinalize(this);
    }
}