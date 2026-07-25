namespace AikoOS.Core.Models;

public sealed class StoredChatMessage
{
    public Guid Id { get; init; }

    public Guid ConversationId { get; init; }

    public string Role { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}