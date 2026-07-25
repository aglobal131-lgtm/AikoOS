using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Context;
using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.State;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Executors;

public sealed class IdleActionExecutor : IActionExecutor
{
    private readonly ICharacterContext _characterContext;
    private readonly ILogger<IdleActionExecutor> _logger;

    public IdleActionExecutor(
        ICharacterContext characterContext,
        ILogger<IdleActionExecutor> logger)
    {
        _characterContext = characterContext;
        _logger = logger;
    }

    public bool CanExecute(CharacterAction action)
    {
        return action.Name ==
               CharacterActionNames.Idle;
    }

    public Task ExecuteAsync(
        CharacterAction action,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string emotion =
            action.Emotion ?? EmotionNames.Neutral;

        _characterContext.Update(
            CharacterStateNames.Idle,
            emotion);

        _logger.LogInformation(
            "Idle action executed. State: {State}, Emotion: {Emotion}, Speech: {Speech}.",
            CharacterStateNames.Idle,
            emotion,
            action.Speech);

        return Task.CompletedTask;
    }
}