using AikoOS.Behavior.Events;

namespace AikoOS.Behavior.Brain;

public interface IBehaviorEngine
{
    Task HandleEventAsync(
        BehaviorEvent behaviorEvent,
        CancellationToken cancellationToken = default);
}