using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Ollama;

internal sealed class OllamaModelInfo
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;
}