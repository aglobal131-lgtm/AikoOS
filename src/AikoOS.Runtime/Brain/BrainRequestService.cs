using AikoOS.AI.Brain.Decisions;
using AikoOS.Runtime.Brain.Models;
using AikoOS.Runtime.Planning;
using AikoOS.Memory.Interfaces;
using AikoOS.Memory.Models;
using Microsoft.Extensions.Logging;

namespace AikoOS.Runtime.Brain;

public sealed class BrainRequestService
    : IBrainRequestService
{
    private readonly IBrainRuntimeService _brainRuntimeService;
    private readonly IMemoryPipeline _memoryPipeline;
    private readonly ILogger<BrainRequestService> _logger;

    public BrainRequestService(
    IBrainRuntimeService brainRuntimeService,
    IMemoryPipeline memoryPipeline,
    ILogger<BrainRequestService> logger)
    {
        _brainRuntimeService = brainRuntimeService;
        _memoryPipeline = memoryPipeline;
        _logger = logger;
    }

    public Task<BrainResponse> ProcessAsync(
        string userInput,
        CancellationToken cancellationToken = default)
    {
        BrainRequest request =
            new()
            {
                UserInput = userInput
            };

        return ProcessAsync(
            request,
            cancellationToken);
    }

    public async Task<BrainResponse> ProcessAsync(
        BrainRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.UserInput))
        {
            _logger.LogWarning(
                "Brain request was ignored because user input was empty.");

            return new BrainResponse
            {
                Success = false,
                ErrorMessage = "Nội dung không được để trống."
            };
        }

        string normalizedInput =
            request.UserInput.Trim();

        MemoryContext memoryContext =
new()
{
    UserInput = normalizedInput
};

        await _memoryPipeline.RecallAsync(
            memoryContext,
            cancellationToken);

        _logger.LogInformation(
            "Processing brain request with {HistoryCount} history message(s).",
            request.ConversationHistory.Count);

        BrainRequest runtimeRequest =
    new()
    {
        UserInput = normalizedInput,
        ConversationHistory = request.ConversationHistory
    };

        Plan plan =
            await _brainRuntimeService.ExecuteAsync(
                runtimeRequest,
                cancellationToken);

        Decision? decision =
            plan.Decisions.FirstOrDefault();

        if (decision is null)
        {
            _logger.LogWarning(
                "Brain returned a plan without decisions.");

            return new BrainResponse
            {
                Success = false,
                ErrorMessage =
                    "Brain không tạo được quyết định phù hợp."
            };
        }

        return new BrainResponse
        {
            Success = true,
            Speech = decision.Speech ?? string.Empty,
            Emotion = decision.Emotion ?? "Neutral",
            Action = decision.Action
        };
    }
}