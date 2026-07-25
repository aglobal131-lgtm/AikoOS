using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Gemini;

internal sealed class GeminiGenerationConfig
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; init; } = 0.7;

    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; init; } = 2048;
}