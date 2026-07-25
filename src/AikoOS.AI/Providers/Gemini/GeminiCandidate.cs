using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Gemini;

internal sealed class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; init; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; init; }
}