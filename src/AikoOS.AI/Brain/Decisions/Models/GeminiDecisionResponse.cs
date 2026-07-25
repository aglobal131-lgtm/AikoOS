namespace AikoOS.AI.Brain.Decisions.Models;

public sealed class GeminiDecisionResponse
{
    public string Emotion { get; init; } = "Neutral";

    public string Action { get; init; } = "Talk";

    public string Speech { get; init; } = string.Empty;
}