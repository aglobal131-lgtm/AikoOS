using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Context;
using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.State;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Executors;

public sealed class SleepActionExecutor : IActionExecutor
{
    private readonly ICharacterContext _characterContext;
    private readonly ILogger<SleepActionExecutor> _logger;

    public SleepActionExecutor(
        ICharacterContext characterContext,
        ILogger<SleepActionExecutor> logger)
    {
        _characterContext = characterContext;
        _logger = logger;
    }

    public bool CanExecute(
        CharacterAction action)
    {
        return action.Name ==
               CharacterActionNames.Sleep;
    }

    public Task ExecuteAsync(
        CharacterAction action,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string emotion =
            action.Emotion ?? EmotionNames.Sleepy;

        _characterContext.Update(
            CharacterStateNames.Sleeping,
            emotion);

        _logger.LogInformation(
            "Sleep action executed. State: {State}, Emotion: {Emotion}, Speech: {Speech}.",
            CharacterStateNames.Sleeping,
            emotion,
            action.Speech);

        return Task.CompletedTask;
    }
}