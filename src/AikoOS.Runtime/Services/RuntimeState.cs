using AikoOS.Runtime.Models;

namespace AikoOS.Runtime.Services;

public sealed class RuntimeState : IRuntimeState
{
    private readonly object _lock = new();

    private RuntimeStatus _status =
        RuntimeStatus.Starting;

    public RuntimeState()
    {
        StartedAt = DateTimeOffset.Now;
    }

    public RuntimeStatus Status
    {
        get
        {
            lock (_lock)
            {
                return _status;
            }
        }
    }

    public DateTimeOffset StartedAt { get; }

    public TimeSpan Uptime =>
        DateTimeOffset.Now - StartedAt;

    public event EventHandler? StateChanged;

    public void SetStatus(RuntimeStatus status)
    {
        bool changed;

        lock (_lock)
        {
            changed = _status != status;

            if (changed)
            {
                _status = status;
            }
        }

        if (changed)
        {
            StateChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }
}