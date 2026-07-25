namespace AikoOS.Runtime.Brain.Models;

public sealed class BrainResponse
{
    public bool Success { get; init; }

    public string Speech { get; init; } = string.Empty;

    public string Emotion { get; init; } = "Neutral";

    public string Action { get; init; } = "Idle";

    public string ErrorMessage { get; init; } = string.Empty;
}