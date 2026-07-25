namespace AikoOS.App.Models;

public sealed class ChatMessageItem
{
    public string Role { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public bool IsUser =>
        string.Equals(
            Role,
            "user",
            StringComparison.OrdinalIgnoreCase);

    public string DisplayName =>
        IsUser ? "Bạn" : "Aiko";
}