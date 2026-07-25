using AikoOS.Behavior.Events;
using AikoOS.Behavior.Actions;
using AikoOS.Behavior.State;

namespace AikoOS.Behavior.Rules;

public interface IBehaviorRule
{
    bool CanHandle(
        BehaviorEvent behaviorEvent,
        CharacterState state);

    Task<CharacterAction?> ExecuteAsync(
        BehaviorEvent behaviorEvent,
        CharacterState state,
        CancellationToken cancellationToken = default);
}