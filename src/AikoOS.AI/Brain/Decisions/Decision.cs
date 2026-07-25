namespace AikoOS.AI.Brain.Decisions;

public sealed class Decision
{
    public required string Action { get; init; }

    public required string Emotion { get; init; }

    public required string Speech { get; init; }

    public Dictionary<string, object?> Parameters { get; init; }
        = new();
}