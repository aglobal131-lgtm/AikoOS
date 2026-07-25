namespace AikoOS.Behavior.Idle;

public interface IIdleMonitor
{
    bool IsUserIdle(
        TimeSpan idleThreshold);

    TimeSpan GetCurrentIdleDuration();
}