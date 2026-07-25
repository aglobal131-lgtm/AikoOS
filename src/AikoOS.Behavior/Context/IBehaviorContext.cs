using AikoOS.Behavior.Context;

namespace AikoOS.Behavior.Context;

public interface IBehaviorContext
{
    BehaviorState Current { get; }

    void UpdateIdleDuration(
        TimeSpan idleDuration);
}