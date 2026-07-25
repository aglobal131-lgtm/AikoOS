using System.Collections.Concurrent;
using AikoOS.Behavior.Actions;

namespace AikoOS.Behavior.Queues;

public sealed class ActionQueue : IActionQueue
{
    private readonly ConcurrentQueue<CharacterAction> _queue = new();

    public void Enqueue(CharacterAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _queue.Enqueue(action);
    }

    public bool TryDequeue(out CharacterAction? action)
    {
        return _queue.TryDequeue(out action);
    }
}