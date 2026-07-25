using AikoOS.Behavior.Events;

namespace AikoOS.Behavior.Bus;

public sealed class BehaviorEventBus : IBehaviorEventBus
{
    private readonly object _lock = new();

    private readonly Dictionary<string, List<Action<BehaviorEvent>>> _handlers = new();

    public void Publish(BehaviorEvent behaviorEvent)
    {
        ArgumentNullException.ThrowIfNull(behaviorEvent);

        List<Action<BehaviorEvent>> handlers;

        lock (_lock)
        {
            if (!_handlers.TryGetValue(behaviorEvent.Name, out var registeredHandlers))
            {
                return;
            }

            handlers = registeredHandlers.ToList();
        }

        foreach (var handler in handlers)
        {
            try
            {
                handler(behaviorEvent);
            }
            catch
            {
                // TODO: Thêm ILogger ở bước sau.
            }
        }
    }

    public IDisposable Subscribe(
        string eventName,
        Action<BehaviorEvent> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);
        ArgumentNullException.ThrowIfNull(handler);

        lock (_lock)
        {
            if (!_handlers.TryGetValue(eventName, out var handlers))
            {
                handlers = new List<Action<BehaviorEvent>>();
                _handlers[eventName] = handlers;
            }

            handlers.Add(handler);
        }

        return new EventSubscription(
            () => Unsubscribe(eventName, handler));
    }

    private void Unsubscribe(
        string eventName,
        Action<BehaviorEvent> handler)
    {
        lock (_lock)
        {
            if (!_handlers.TryGetValue(eventName, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                _handlers.Remove(eventName);
            }
        }
    }
}