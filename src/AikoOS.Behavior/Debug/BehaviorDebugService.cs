using AikoOS.Behavior.Context;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Debug;

public sealed class BehaviorDebugService
{
    private readonly ICharacterContext _characterContext;
    private readonly ILogger<BehaviorDebugService> _logger;

    public BehaviorDebugService(
        ICharacterContext characterContext,
        ILogger<BehaviorDebugService> logger)
    {
        _characterContext = characterContext;
        _logger = logger;
    }

    public void DumpRuntimeState()
    {
        CharacterRuntimeState state =
            _characterContext.Current;

        _logger.LogInformation(
            """
            ========= Behavior =========
            State     : {State}
            Emotion   : {Emotion}
            UpdatedAt : {UpdatedAt}
            ============================
            """,
            state.StateName,
            state.EmotionName,
            state.UpdatedAt);
    }
}