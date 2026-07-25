namespace AikoOS.Behavior.Events;

public sealed class BehaviorEvent
{
    public required string Name { get; init; }

    public Dictionary<string, object?> Data { get; init; } = new();

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}