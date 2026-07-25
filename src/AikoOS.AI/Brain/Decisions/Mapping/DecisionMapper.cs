using AikoOS.AI.Brain.Decisions.Models;

namespace AikoOS.AI.Brain.Decisions.Mapping;

public static class DecisionMapper
{
    public static Decision ToDecision(
        GeminiDecisionResponse response)
    {
        return new Decision
        {
            Action = NormalizeAction(response.Action),
            Emotion = NormalizeEmotion(response.Emotion),
            Speech = response.Speech.Trim()
        };
    }

    private static string NormalizeAction(
        string? action)
    {
        return action?.Trim().ToLowerInvariant() switch
        {
            "idle" => "Idle",
            "talk" => "Talk",
            "wave" => "Wave",
            "think" => "Think",
            "sleep" => "Sleep",
            _ => "Talk"
        };
    }

    private static string NormalizeEmotion(
        string? emotion)
    {
        return emotion?.Trim().ToLowerInvariant() switch
        {
            "neutral" => "Neutral",
            "happy" => "Happy",
            "sad" => "Sad",
            "sleepy" => "Sleepy",
            "curious" => "Curious",
            "excited" => "Excited",
            _ => "Neutral"
        };
    }
}