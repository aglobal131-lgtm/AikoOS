namespace AikoOS.Memory.Models;

public sealed class MemoryContext
{
    public string UserInput { get; init; } = string.Empty;

    public string AssistantResponse { get; set; } = string.Empty;

    public IReadOnlyList<string> RelevantMemories { get; set; }
        = [];
}