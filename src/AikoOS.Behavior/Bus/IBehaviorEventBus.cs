using AikoOS.Behavior.Events;

namespace AikoOS.Behavior.Bus;

public interface IBehaviorEventBus
{
    void Publish(BehaviorEvent behaviorEvent);

    IDisposable Subscribe(
        string eventName,
        Action<BehaviorEvent> handler);
}