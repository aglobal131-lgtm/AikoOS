using AikoOS.Behavior.Context;
using AikoOS.Core.Models;
using AikoOS.Core.Services;
using Microsoft.Extensions.Logging;

namespace AikoOS.App.Avatar;

public sealed class BehaviorAvatarSyncService
    : IDisposable
{
    private readonly ICharacterContext
        _characterContext;

    private readonly IAikoStateService
        _stateService;

    private readonly IAvatarStateMapper
        _avatarStateMapper;

    private readonly ILogger<BehaviorAvatarSyncService>
        _logger;

    private bool _isDisposed;

    public BehaviorAvatarSyncService(
        ICharacterContext characterContext,
        IAikoStateService stateService,
        IAvatarStateMapper avatarStateMapper,
        ILogger<BehaviorAvatarSyncService> logger)
    {
        _characterContext = characterContext;
        _stateService = stateService;
        _avatarStateMapper = avatarStateMapper;
        _logger = logger;

        _characterContext.StateChanged +=
            CharacterContext_StateChanged;

        _logger.LogInformation(
            "BehaviorAvatarSyncService was created.");
    }

    private void CharacterContext_StateChanged(
        CharacterRuntimeState state)
    {
        try
        {
            AikoAvatarState avatarState =
                _avatarStateMapper.Map(state);

            _stateService.SetState(
                avatarState);

            _logger.LogDebug(
                "Behavior state synchronized to avatar. State: {State}, Emotion: {Emotion}.",
                state.StateName,
                state.EmotionName);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to synchronize behavior state to avatar.");
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _characterContext.StateChanged -=
            CharacterContext_StateChanged;

        _isDisposed = true;

        _logger.LogInformation(
            "BehaviorAvatarSyncService was disposed.");
    }
}