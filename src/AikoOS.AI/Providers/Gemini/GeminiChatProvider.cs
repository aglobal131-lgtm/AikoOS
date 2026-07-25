using System.Net.Http.Json;
using System.Text.Json;
using AikoOS.AI.Interfaces;
using AikoOS.AI.Models;
using AikoOS.AI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AikoOS.AI.Providers.Gemini;

public sealed class GeminiChatProvider : IChatProvider
{
    private readonly HttpClient _httpClient;
    private readonly AIOptions _options;
    private readonly ILogger<GeminiChatProvider> _logger;

    public GeminiChatProvider(
        HttpClient httpClient,
        IOptions<AIOptions> options,
        ILogger<GeminiChatProvider> logger)
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
                ErrorMessage =
                    "At least one chat message is required."
            };
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "Gemini API key is not configured."
            };
        }

        GeminiGenerateContentRequest request =
            CreateRequest(messages);

        string requestUri =
            $"v1beta/models/{Uri.EscapeDataString(_options.Model)}:generateContent";

        using HttpRequestMessage httpRequest =
            new(HttpMethod.Post, requestUri);

        httpRequest.Headers.TryAddWithoutValidation(
            "x-goog-api-key",
            _options.ApiKey);

        httpRequest.Content =
            JsonContent.Create(request);

        try
        {
            _logger.LogInformation(
                "Sending request to Gemini model {Model}.",
                _options.Model);

            using HttpResponseMessage response =
                await _httpClient.SendAsync(
                    httpRequest,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                string errorMessage =
                    ParseErrorMessage(errorBody);

                _logger.LogWarning(
                    "Gemini returned HTTP {StatusCode}: {ErrorMessage}",
                    (int)response.StatusCode,
                    errorMessage);

                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage =
                        $"Gemini error: {errorMessage}"
                };
            }

            GeminiGenerateContentResponse? result =
                await response.Content
                    .ReadFromJsonAsync<
                        GeminiGenerateContentResponse>(
                        cancellationToken:
                            cancellationToken);

            string content =
                result?
                    .Candidates
                    .FirstOrDefault()?
                    .Content?
                    .Parts
                    .Select(part => part.Text)
                    .FirstOrDefault(text =>
                        !string.IsNullOrWhiteSpace(text))
                    ?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(content))
            {
                return new ChatResponse
                {
                    Success = false,
                    ErrorMessage =
                        "Gemini returned an empty response."
                };
            }

            _logger.LogInformation(
                "Gemini response received successfully.");

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
                "Could not connect to Gemini.");

            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "Could not connect to Gemini. Check your internet connection."
            };
        }
        catch (TaskCanceledException exception)
            when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(
                exception,
                "The Gemini request timed out.");

            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "The Gemini request timed out."
            };
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "Gemini returned invalid JSON.");

            return new ChatResponse
            {
                Success = false,
                ErrorMessage =
                    "Gemini returned an invalid response."
            };
        }
    }

    private static GeminiGenerateContentRequest CreateRequest(
        IReadOnlyList<ChatMessage> messages)
    {
        ChatMessage[] systemMessages =
            messages
                .Where(message =>
                    string.Equals(
                        message.Role,
                        "system",
                        StringComparison.OrdinalIgnoreCase))
                .ToArray();

        GeminiContent? systemInstruction = null;

        if (systemMessages.Length > 0)
        {
            systemInstruction = new GeminiContent
            {
                Parts = systemMessages
                    .Select(message => new GeminiPart
                    {
                        Text = message.Content
                    })
                    .ToArray()
            };
        }

        GeminiContent[] contents =
            messages
                .Where(message =>
                    !string.Equals(
                        message.Role,
                        "system",
                        StringComparison.OrdinalIgnoreCase))
                .Select(message => new GeminiContent
                {
                    Role = ConvertRole(message.Role),
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = message.Content
                        }
                    ]
                })
                .ToArray();

        return new GeminiGenerateContentRequest
        {
            SystemInstruction = systemInstruction,
            Contents = contents,
            GenerationConfig =
                new GeminiGenerationConfig
                {
                    Temperature = 0.7,
                    MaxOutputTokens = 2048
                }
        };
    }

    private static string ConvertRole(string role)
    {
        return string.Equals(
            role,
            "assistant",
            StringComparison.OrdinalIgnoreCase)
                ? "model"
                : "user";
    }

    private static string ParseErrorMessage(
        string errorBody)
    {
        try
        {
            GeminiErrorResponse? response =
                JsonSerializer.Deserialize<
                    GeminiErrorResponse>(
                    errorBody);

            return string.IsNullOrWhiteSpace(
                response?.Error?.Message)
                    ? errorBody
                    : response.Error.Message;
        }
        catch (JsonException)
        {
            return errorBody;
        }
    }
}