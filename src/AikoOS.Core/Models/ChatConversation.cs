namespace AikoOS.Core.Models;

public sealed class ChatConversation
{
    public Guid Id { get; init; }

    public string Title { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; set; }
}