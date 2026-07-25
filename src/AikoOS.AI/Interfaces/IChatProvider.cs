using AikoOS.AI.Models;

namespace AikoOS.AI.Interfaces;

public interface IChatProvider
{
    Task<ChatResponse> SendAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken = default);
}