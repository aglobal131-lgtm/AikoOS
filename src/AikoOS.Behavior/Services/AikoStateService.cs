using AikoOS.Core.Models;
using AikoOS.Core.Services;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Services;

public sealed class AikoStateService : IAikoStateService
{
    private readonly ILogger<AikoStateService> _logger;
    private readonly object _stateLock = new();

    private AikoAvatarState _currentState = new();

    public AikoStateService(
        ILogger<AikoStateService> logger)
    {
        _logger = logger;
    }

    public AikoAvatarState CurrentState
    {
        get
        {
            lock (_stateLock)
            {
                return _currentState;
            }
        }
    }

    public event EventHandler<AikoAvatarState>?
        StateChanged;

    public void SetState(AikoAvatarState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        AikoAvatarState updatedState = state with
        {
            UpdatedAt = DateTimeOffset.UtcNow
        };

        lock (_stateLock)
        {
            _currentState = updatedState;
        }

        _logger.LogInformation(
            "Aiko state changed. Emotion: {Emotion}, " +
            "Motion: {MotionGroup}/{MotionIndex}, " +
            "Expression: {ExpressionName}.",
            updatedState.Emotion,
            updatedState.MotionGroup,
            updatedState.MotionIndex,
            updatedState.ExpressionName);

        StateChanged?.Invoke(
            this,
            updatedState);
    }

    public void SetEmotion(AikoEmotion emotion)
    {
        AikoAvatarState updatedState;

        lock (_stateLock)
        {
            updatedState = _currentState with
            {
                Emotion = emotion,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _currentState = updatedState;
        }

        _logger.LogInformation(
            "Aiko emotion changed to {Emotion}.",
            emotion);

        StateChanged?.Invoke(
            this,
            updatedState);
    }

    public void Reset()
    {
        SetState(new AikoAvatarState());
    }
}