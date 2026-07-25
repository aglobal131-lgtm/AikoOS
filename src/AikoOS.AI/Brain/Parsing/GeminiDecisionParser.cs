using System.Text.Json;
using AikoOS.AI.Brain.Decisions.Models;
using AikoOS.AI.Brain.Decisions.Validation;
using Microsoft.Extensions.Logging;

namespace AikoOS.AI.Brain.Parsing;

public sealed class GeminiDecisionParser
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    private readonly ILogger<GeminiDecisionParser> _logger;

    public GeminiDecisionParser(
        ILogger<GeminiDecisionParser> logger)
    {
        _logger = logger;
    }

    public GeminiDecisionResponse? Parse(
        string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogWarning(
                "Cannot parse an empty Gemini response.");

            return null;
        }

        string normalizedContent =
            RemoveMarkdownCodeBlock(content);

        try
        {
            GeminiDecisionResponse? result =
                JsonSerializer.Deserialize<GeminiDecisionResponse>(
                    normalizedContent,
                    JsonOptions);

            if (result is null)
            {
                _logger.LogWarning(
                    "Gemini decision JSON was deserialized to null.");

                return null;
            }

            if (!DecisionValidator.IsValid(result))
            {
                _logger.LogWarning(
                    "Gemini decision JSON contains invalid values. Content: {Content}",
                    normalizedContent);

                return null;
            }

            return result;
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(
                exception,
                "Gemini returned invalid decision JSON: {Content}",
                normalizedContent);

            return null;
        }
    }

    private static string RemoveMarkdownCodeBlock(
        string content)
    {
        string result = content.Trim();

        if (!result.StartsWith(
                "```",
                StringComparison.Ordinal))
        {
            return result;
        }

        int firstLineEnd =
            result.IndexOf('\n');

        if (firstLineEnd >= 0)
        {
            result =
                result[(firstLineEnd + 1)..];
        }

        if (result.EndsWith(
                "```",
                StringComparison.Ordinal))
        {
            result =
                result[..^3];
        }

        return result.Trim();
    }
}