using AikoOS.Core.Models;
using AikoOS.Core.Services;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Emotion;

public sealed class EmotionEngine : IEmotionEngine
{
    private readonly IAikoStateService _stateService;
    private readonly ILogger<EmotionEngine> _logger;

    public EmotionEngine(
        IAikoStateService stateService,
        ILogger<EmotionEngine> logger)
    {
        _stateService = stateService;
        _logger = logger;
    }

    public AikoEmotion CurrentEmotion
        => _stateService.CurrentState.Emotion;

    public void SetEmotion(
        AikoEmotion emotion)
    {
        AikoAvatarState state =
            emotion switch
            {
                AikoEmotion.Happy =>
                    new()
                    {
                        Emotion = emotion,
                        ExpressionName = "exp_01",
                        MotionGroup = "TapBody",
                        MotionIndex = 0,
                        LookX = 0.5,
                        LookY = 0
                    },

                AikoEmotion.Sleepy =>
                    new()
                    {
                        Emotion = emotion,
                        ExpressionName = "exp_06",
                        MotionGroup = "Idle",
                        MotionIndex = 0,
                        LookX = 0,
                        LookY = -0.3
                    },

                _ =>
                    new()
                    {
                        Emotion = emotion,
                        ExpressionName = "exp_02",
                        MotionGroup = "Idle",
                        MotionIndex = 0
                    }
            };

        _stateService.SetState(state);

        _logger.LogInformation(
            "Emotion changed to {Emotion}.",
            emotion);
    }
}