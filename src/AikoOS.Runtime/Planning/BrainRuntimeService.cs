using AikoOS.AI.Brain.Decisions;
using AikoOS.AI.Brain.Planning;
using AikoOS.Behavior.Context;
using AikoOS.Runtime.Brain.Models;
using Microsoft.Extensions.Logging;

namespace AikoOS.Runtime.Planning;

public sealed class BrainRuntimeService
    : IBrainRuntimeService
{
    private readonly IPlanningService _planningService;
    private readonly PlanExecutionService _planExecutionService;

    private readonly ICharacterContext _characterContext;
    private readonly ILogger<BrainRuntimeService> _logger;

    public BrainRuntimeService(
    IPlanningService planningService,
    PlanExecutionService planExecutionService,
    ICharacterContext characterContext,
    ILogger<BrainRuntimeService> logger)
    {
        _planningService = planningService;
        _planExecutionService = planExecutionService;
        _characterContext = characterContext;
        _logger = logger;
    }

    public async Task<Plan> ExecuteAsync(
    BrainRequest request,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        PlanningContext planningContext =
    new()
    {
        CurrentState =
            _characterContext.Current.StateName,

        CurrentEmotion =
            _characterContext.Current.EmotionName,

        UserInput = request.UserInput,
        ConversationHistory = request.ConversationHistory,

        Timestamp = DateTime.UtcNow
    };

        _logger.LogInformation(
            "Creating plan. State: {State}, Emotion: {Emotion}, UserInput: {UserInput}.",
            planningContext.CurrentState,
            planningContext.CurrentEmotion,
            planningContext.UserInput);

        Plan plan =
            await _planningService.CreatePlanAsync(
                planningContext,
                cancellationToken);

        _logger.LogInformation(
            "Plan created with {DecisionCount} decision(s).",
            plan.Decisions.Count);

        _planExecutionService.Execute(plan);

        return plan;
    }
}