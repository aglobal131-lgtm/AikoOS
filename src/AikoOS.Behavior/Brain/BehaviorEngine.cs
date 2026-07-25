using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Events;
using AikoOS.Behavior.Queues;
using AikoOS.Behavior.Rules;
using AikoOS.Behavior.State;

namespace AikoOS.Behavior.Brain;

public sealed class BehaviorEngine : IBehaviorEngine
{
    private readonly IReadOnlyList<IBehaviorRule> _rules;
    private readonly IActionQueue _actionQueue;
    private readonly CharacterState _state;

    public BehaviorEngine(
        IEnumerable<IBehaviorRule> rules,
        IActionQueue actionQueue,
        CharacterState state)
    {
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(actionQueue);
        ArgumentNullException.ThrowIfNull(state);

        _rules = rules.ToArray();
        _actionQueue = actionQueue;
        _state = state;
    }

    public async Task HandleEventAsync(
        BehaviorEvent behaviorEvent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(behaviorEvent);

        cancellationToken.ThrowIfCancellationRequested();

        foreach (IBehaviorRule rule in _rules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!rule.CanHandle(behaviorEvent, _state))
            {
                continue;
            }

            CharacterAction? action =
                await rule.ExecuteAsync(
                    behaviorEvent,
                    _state,
                    cancellationToken);

            if (action is null)
            {
                continue;
            }

            _actionQueue.Enqueue(action);
            break;
        }
    }
}