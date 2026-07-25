using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Ollama;

internal sealed class OllamaChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;

    [JsonPropertyName("messages")]
    public IReadOnlyList<OllamaMessage> Messages { get; init; }
        = [];

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}