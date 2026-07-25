using System.Text.Json.Serialization;

namespace AikoOS.AI.Providers.Ollama;

internal sealed class OllamaChatApiResponse
{
    [JsonPropertyName("message")]
    public OllamaMessage? Message { get; init; }

    [JsonPropertyName("done")]
    public bool Done { get; init; }

    [JsonPropertyName("done_reason")]
    public string? DoneReason { get; init; }
}