using AikoOS.AI.Brain.Decisions.Models;

namespace AikoOS.AI.Brain.Decisions.Validation;

public static class DecisionValidator
{
    private static readonly HashSet<string> AllowedActions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Idle",
            "Talk",
            "Wave",
            "Think",
            "Sleep"
        };

    private static readonly HashSet<string> AllowedEmotions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Neutral",
            "Happy",
            "Sad",
            "Sleepy",
            "Curious",
            "Excited"
        };

    public static bool IsValid(
        GeminiDecisionResponse response)
    {
        if (response is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.Speech))
        {
            return false;
        }

        if (!AllowedActions.Contains(response.Action))
        {
            return false;
        }

        if (!AllowedEmotions.Contains(response.Emotion))
        {
            return false;
        }

        return true;
    }
}