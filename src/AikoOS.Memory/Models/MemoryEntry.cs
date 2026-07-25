namespace AikoOS.Memory.Models;

public sealed class MemoryEntry
{
    public long Id { get; init; }

    public string Content { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}