namespace AikoOS.AI.Models;

public sealed class ChatResponse
{
    public bool Success { get; init; }

    public string Content { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}