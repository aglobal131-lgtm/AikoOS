using AikoOS.Core.Models;

namespace AikoOS.Core.Interfaces;

public interface IChatRepository
{
    Task<ChatConversation> CreateConversationAsync(
        string title,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatConversation>>
        GetConversationsAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StoredChatMessage>>
        GetMessagesAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default);

    Task<StoredChatMessage> AddMessageAsync(
        Guid conversationId,
        string role,
        string content,
        CancellationToken cancellationToken = default);

    Task UpdateConversationTitleAsync(
        Guid conversationId,
        string title,
        CancellationToken cancellationToken = default);

    Task DeleteConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);
}