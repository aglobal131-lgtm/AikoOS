using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Context;
using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.State;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Executors;

public sealed class TalkActionExecutor
    : IActionExecutor
{
    private readonly ICharacterContext _characterContext;
    private readonly ILogger<TalkActionExecutor> _logger;

    public TalkActionExecutor(
        ICharacterContext characterContext,
        ILogger<TalkActionExecutor> logger)
    {
        _characterContext = characterContext;
        _logger = logger;
    }

    public bool CanExecute(
        CharacterAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return string.Equals(
            action.Name,
            CharacterActionNames.Talk,
            StringComparison.OrdinalIgnoreCase);
    }

    public Task ExecuteAsync(
        CharacterAction action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        cancellationToken.ThrowIfCancellationRequested();

        string emotion =
            string.IsNullOrWhiteSpace(action.Emotion)
                ? EmotionNames.Neutral
                : action.Emotion;

        _characterContext.Update(
            CharacterStateNames.Speaking,
            emotion);

        _logger.LogInformation(
            "Talk action executed. State: {State}, Emotion: {Emotion}, Speech: {Speech}.",
            CharacterStateNames.Speaking,
            emotion,
            action.Speech);

        return Task.CompletedTask;
    }
}