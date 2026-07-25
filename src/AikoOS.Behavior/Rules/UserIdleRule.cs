using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.Events;
using AikoOS.Behavior.State;

namespace AikoOS.Behavior.Rules;

public sealed class UserIdleRule : IBehaviorRule
{
    public bool CanHandle(
        BehaviorEvent behaviorEvent,
        CharacterState state)
    {
        return behaviorEvent.Name ==
               BehaviorEventNames.UserIdle;
    }

    public Task<CharacterAction?> ExecuteAsync(
        BehaviorEvent behaviorEvent,
        CharacterState state,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        CharacterAction action = new()
        {
            Name = CharacterActionNames.Sleep,
            Emotion = EmotionNames.Sleepy,
            Speech = "Buồn ngủ quá..."
        };

        return Task.FromResult<CharacterAction?>(action);
    }
}