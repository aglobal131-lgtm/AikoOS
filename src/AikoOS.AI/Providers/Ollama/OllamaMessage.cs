using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Ollama;

internal sealed class OllamaMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}