namespace AikoOS.Memory.Models;

public sealed class MemoryRecord
{
    public Guid Id { get; init; }

    public string Content { get; init; } = string.Empty;

    public string Type { get; init; } = "General";

    public double Importance { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}