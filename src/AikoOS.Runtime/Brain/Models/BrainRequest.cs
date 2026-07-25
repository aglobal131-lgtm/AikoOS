using AikoOS.AI.Models;

namespace AikoOS.Runtime.Brain.Models;

public sealed class BrainRequest
{
    public required string UserInput { get; init; }

    public IReadOnlyList<ChatMessage> ConversationHistory { get; init; }
        = [];
}