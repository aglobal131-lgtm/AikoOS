using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Context;
using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.State;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Executors;

public sealed class WakeUpActionExecutor : IActionExecutor
{
    private readonly ICharacterContext _characterContext;
    private readonly ILogger<WakeUpActionExecutor> _logger;

    public WakeUpActionExecutor(
        ICharacterContext characterContext,
        ILogger<WakeUpActionExecutor> logger)
    {
        _characterContext = characterContext;
        _logger = logger;
    }

    public bool CanExecute(CharacterAction action)
    {
        return action.Name ==
               CharacterActionNames.WakeUp;
    }

    public Task ExecuteAsync(
        CharacterAction action,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _characterContext.Update(
            CharacterStateNames.Idle,
            action.Emotion ?? EmotionNames.Neutral);

        _logger.LogInformation(
            "Wake-up action executed. State: {State}, Emotion: {Emotion}, Speech: {Speech}.",
            CharacterStateNames.Idle,
            action.Emotion ?? EmotionNames.Neutral,
            action.Speech);

        return Task.CompletedTask;
    }
}