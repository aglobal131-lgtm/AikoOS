using AikoOS.Runtime.Models;

namespace AikoOS.Runtime.Services;

public interface IRuntimeState
{
    RuntimeStatus Status { get; }

    DateTimeOffset StartedAt { get; }

    TimeSpan Uptime { get; }

    event EventHandler? StateChanged;

    void SetStatus(RuntimeStatus status);
}