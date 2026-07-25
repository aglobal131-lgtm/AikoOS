using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Gemini;

internal sealed class GeminiGenerateContentResponse
{
    [JsonPropertyName("candidates")]
    public IReadOnlyList<GeminiCandidate> Candidates { get; init; }
        = [];
}