using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Events;
using AikoOS.Behavior.Queues;
using AikoOS.Behavior.Rules;
using AikoOS.Behavior.State;

namespace AikoOS.Behavior.Brain;

public sealed class BehaviorEngine : IBehaviorEngine
{
    private readonly IEnumerable<IBehaviorRule> _rules;
    private readonly IActionQueue _actionQueue;

    private readonly CharacterState _state = new();

    public BehaviorEngine(
        IEnumerable<IBehaviorRule> rules,
        IActionQueue actionQueue)
    {
        _rules = rules;
        _actionQueue = actionQueue;
    }

    public async Task HandleEventAsync(
        BehaviorEvent behaviorEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(behaviorEvent);

        foreach (var rule in _rules)
        {
            if (!rule.CanHandle(behaviorEvent, _state))
            {
                continue;
            }

            CharacterAction? action =
                await rule.ExecuteAsync(
                    behaviorEvent,
                    _state,
                    cancellationToken);

            if (action is not null)
            {
                _actionQueue.Enqueue(action);
                break;
            }
        }
    }
}