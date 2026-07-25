namespace AikoOS.Behavior.Actions;

public sealed class CharacterAction
{
    public required string Name { get; init; }

    public string? Emotion { get; init; }

    public string? Speech { get; init; }

    public Dictionary<string, object?> Parameters { get; init; } = new();
}