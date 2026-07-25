using System.Net.Http.Json;
using System.Text.Json;
using AikoOS.AI.Interfaces;
using AikoOS.AI.Models;
using AikoOS.AI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AikoOS.AI.Providers.Ollama;

public sealed class OllamaChatProvider : IChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly AIOptions _options;
    private readonly ILogger<OllamaChatProvider> _logger;

    public OllamaChatProvider(
        HttpClient httpClient,
        IOptions<AIOptions> options,
        ILogger<OllamaChatProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ChatResponse> SendAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        if (messages.Count == 0)
        {
            return new ChatResponse
            {
                Success = false,
                ErrorMessage = "At least one chat message is required."
            };
        }

        OllamaChatRequest request = new()
        {
            Model = _options.Model,
            Stream = false,
            Messages = messages
                .Select(message => new OllamaMessage
                {
                    Role = message.Role,
                    Content = message.Content
                })
                .ToArray()
        };

        try
        {
            _logger.LogInformation(
                "Sending chat request to Ollama model {Model}.",
                _options.Model);

            using HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "api/chat",
                    request,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                _logger.LogWarning(
                    "Ollama returned HTTP {StatusCode}: {ErrorBody}",
                    (int)response.StatusCode,
                    errorBody);

                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage =
                        $"Ollama returned HTTP {(int)response.StatusCode}."
                };
            }

            OllamaChatApiResponse? ollamaResponse =
                await response.Content.ReadFromJsonAsync<
                    OllamaChatApiResponse>(
                    cancellationToken: cancellationToken);

            string content =
                ollamaResponse?.Message?.Content?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(content))
            {
                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage =
                        "Ollama returned an empty response."
                };
            }

            _logger.LogInformation(
                "Ollama chat response received successfully.");

            return new ChatResponse
            {
                Success = true,
                Content = content
            };
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "Could not connect to Ollama at {Endpoint}.",
                _options.Endpoint);

            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "Could not connect to Ollama. Make sure Ollama is running."
            };
        }
        catch (TaskCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                exception,
                "The Ollama request timed out.");

            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "The Ollama request timed out."
            };
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "Ollama returned invalid JSON.");

            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "Ollama returned an invalid response."
            };
        }
    }
}