namespace AikoOS.AI.Brain.Planning;

using AikoOS.AI.Models;

public sealed class PlanningContext
{
    public string CurrentState { get; init; } = string.Empty;

    public string CurrentEmotion { get; init; } = string.Empty;

    public string? UserInput { get; init; }

    public IReadOnlyList<ChatMessage> ConversationHistory { get; init; }
    = [];

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}