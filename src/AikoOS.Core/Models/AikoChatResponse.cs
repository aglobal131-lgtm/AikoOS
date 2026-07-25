namespace AikoOS.Core.Models;

public sealed class AikoChatResponse
{
    public string JapaneseText { get; init; } = string.Empty;

    public string VietnameseText { get; init; } = string.Empty;

    public string Emotion { get; init; } = "neutral";
}