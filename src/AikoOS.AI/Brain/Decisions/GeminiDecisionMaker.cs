using AikoOS.AI.Brain.Decisions.Models;
using AikoOS.AI.Brain.Parsing;
using AikoOS.AI.Brain.Planning;
using AikoOS.AI.Brain.Prompting;
using AikoOS.AI.Interfaces;
using AikoOS.AI.Models;
using AikoOS.AI.Brain.Decisions.Mapping;
using Microsoft.Extensions.Logging;

namespace AikoOS.AI.Brain.Decisions;

public sealed class GeminiDecisionMaker
    : IDecisionMaker
{
    private readonly IChatProvider _chatProvider;
    private readonly GeminiDecisionParser _parser;
    private readonly ILogger<GeminiDecisionMaker> _logger;

    public GeminiDecisionMaker(
     IChatProvider chatProvider,
     GeminiDecisionParser parser,
     ILogger<GeminiDecisionMaker> logger)
    {
        _chatProvider = chatProvider;
        _parser = parser;
        _logger = logger;
    }

    public async Task<Plan> DecideAsync(
        PlanningContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(context.UserInput))
        {
            _logger.LogWarning(
                "Gemini decision was skipped because user input was empty.");

            return new Plan();
        }

        List<ChatMessage> messages =
[
    new ChatMessage
    {
        Role = "system",
        Content =
            PromptBuilder.BuildSystemPrompt(
                new PromptContext
                {
                    PlanningContext = context,
                    UserName = "User",
                    AssistantName = "Aiko"
                })
    }
];

        messages.AddRange(
            context.ConversationHistory
                .Where(message =>
                    message.Role is "user" or "assistant")
                .Where(message =>
                    !string.IsNullOrWhiteSpace(message.Content))
                .Select(message =>
                    new ChatMessage
                    {
                        Role = message.Role,
                        Content = message.Content
                    }));

        messages.Add(
            new ChatMessage
            {
                Role = "user",
                Content =
                    BuildUserMessage(context)
            });

        ChatResponse response =
            await _chatProvider.SendAsync(
                messages,
                cancellationToken);

        if (!response.Success)
        {
            _logger.LogWarning(
                "Gemini request failed: {ErrorMessage}",
                response.ErrorMessage);

            return CreateFallbackPlan(
                "Xin lỗi, hiện tại mình chưa thể trả lời.");
        }

        GeminiDecisionResponse? result =
            _parser.Parse(response.Content);

        if (result is null)
        {
            return CreateFallbackPlan(
                response.Content);
        }

        Decision decision =
    DecisionMapper.ToDecision(result);

        _logger.LogInformation(
        "Gemini decision created. Action: {Action}, Emotion: {Emotion}, Speech: {Speech}",
        decision.Action,
        decision.Emotion,
        decision.Speech);

        return new Plan
        {
            Decisions =
            [
                decision
            ]
        };
    }

    private static string BuildUserMessage(
        PlanningContext context)
    {
        return $"""
Current state: {context.CurrentState}
Current emotion: {context.CurrentEmotion}
Current time: {context.Timestamp:O}

User message:
{context.UserInput}
""";
    }

    private static Plan CreateFallbackPlan(
        string speech)
    {
        return new Plan
        {
            Decisions =
            [
                new Decision
                {
                    Action = "Talk",
                    Emotion = "Neutral",
                    Speech = speech
                }
            ]
        };
    }
}