namespace AikoOS.Behavior.Context;

public sealed class BehaviorContext : IBehaviorContext
{
    private readonly object _syncRoot = new();

    private BehaviorState _current = new()
    {
        IdleDuration = TimeSpan.Zero,
        Timestamp = DateTime.UtcNow
    };

    public BehaviorState Current
    {
        get
        {
            lock (_syncRoot)
            {
                return _current;
            }
        }
    }

    public void UpdateIdleDuration(
        TimeSpan idleDuration)
    {
        if (idleDuration < TimeSpan.Zero)
        {
            idleDuration = TimeSpan.Zero;
        }

        lock (_syncRoot)
        {
            _current = new BehaviorState
            {
                IdleDuration = idleDuration,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}