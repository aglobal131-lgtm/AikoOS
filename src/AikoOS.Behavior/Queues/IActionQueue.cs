using AikoOS.Behavior.Actions;

namespace AikoOS.Behavior.Queues;

public interface IActionQueue
{
    void Enqueue(CharacterAction action);

    bool TryDequeue(out CharacterAction? action);
}