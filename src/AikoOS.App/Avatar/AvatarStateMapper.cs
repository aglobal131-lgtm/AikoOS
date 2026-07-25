using AikoOS.Behavior.Context;
using AikoOS.Core.Models;

namespace AikoOS.App.Avatar;

public sealed class AvatarStateMapper
    : IAvatarStateMapper
{
    public AikoAvatarState Map(
        CharacterRuntimeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new AikoAvatarState
        {
            Emotion = ParseEmotion(
                state.EmotionName),

            MotionGroup = MapMotionGroup(
                state.StateName),

            MotionIndex = 0,

            ExpressionName = MapExpression(
                state.EmotionName),

            LookX = 0f,
            LookY = 0f
        };
    }

    private static AikoEmotion ParseEmotion(
        string? emotionName)
    {
        if (string.IsNullOrWhiteSpace(
                emotionName))
        {
            return AikoEmotion.Neutral;
        }

        return Enum.TryParse(
            emotionName,
            ignoreCase: true,
            out AikoEmotion emotion)
                ? emotion
                : AikoEmotion.Neutral;
    }

    private static string MapMotionGroup(
        string? stateName)
    {
        /*
         * Model Mao hiện chỉ hỗ trợ:
         * - Idle
         * - TapBody
         *
         * Vì vậy các trạng thái tự động hiện tại
         * đều dùng motion Idle.
         */

        return "Idle";
    }

    private static string MapExpression(
        string? emotionName)
    {
        if (string.IsNullOrWhiteSpace(
                emotionName))
        {
            return "exp_01";
        }

        return emotionName
            .Trim()
            .ToLowerInvariant() switch
        {
            "neutral" => "exp_01",
            "happy" => "exp_02",
            "sad" => "exp_03",
            "angry" => "exp_04",
            "surprised" => "exp_05",
            "sleepy" => "exp_06",
            _ => "exp_01"
        };
    }
}