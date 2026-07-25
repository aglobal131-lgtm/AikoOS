namespace AikoOS.AI.Models;

public sealed class ChatMessage
{
    public string Role { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;
}