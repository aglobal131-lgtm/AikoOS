using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Gemini;

internal sealed class GeminiContent
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("parts")]
    public IReadOnlyList<GeminiPart> Parts { get; init; }
        = [];
}