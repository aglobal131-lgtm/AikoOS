using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Gemini;

internal sealed class GeminiGenerateContentRequest
{
    [JsonPropertyName("systemInstruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiContent? SystemInstruction { get; init; }

    [JsonPropertyName("contents")]
    public IReadOnlyList<GeminiContent> Contents { get; init; }
        = [];

    [JsonPropertyName("generationConfig")]
    public GeminiGenerationConfig GenerationConfig { get; init; }
        = new();
}