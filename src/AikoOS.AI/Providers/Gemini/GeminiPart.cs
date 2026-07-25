using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Gemini;

internal sealed class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;
}