using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Ollama;

internal sealed class OllamaTagsResponse
{
    [JsonPropertyName("models")]
    public IReadOnlyList<OllamaModelInfo> Models { get; init; }
        = [];
}