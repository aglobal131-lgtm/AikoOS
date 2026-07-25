namespace AikoOS.Behavior.Idle;

public sealed class IdleMonitor : IIdleMonitor
{
    private readonly IUserIdleDetector _idleDetector;

    public IdleMonitor(
        IUserIdleDetector idleDetector)
    {
        _idleDetector = idleDetector;
    }

    public bool IsUserIdle(
        TimeSpan idleThreshold)
    {
        return _idleDetector.GetIdleDuration()
            >= idleThreshold;
    }

    public TimeSpan GetCurrentIdleDuration()
    {
        return _idleDetector.GetIdleDuration();
    }
}