using AikoOS.AI.Brain.Decisions;
using AikoOS.Behavior.Queues;
using AikoOS.Behavior.Actions;
using Microsoft.Extensions.Logging;

namespace AikoOS.Runtime.Planning;

public sealed class PlanExecutionService
{
    private readonly IPlanActionMapper _mapper;
    private readonly IActionQueue _actionQueue;
    private readonly ILogger<PlanExecutionService> _logger;

    public PlanExecutionService(
    IPlanActionMapper mapper,
    IActionQueue actionQueue,
    ILogger<PlanExecutionService> logger)
    {
        _mapper = mapper;
        _actionQueue = actionQueue;
        _logger = logger;
    }

    public void Execute(
        Plan plan)
    {
        IReadOnlyList<CharacterAction> actions =
            _mapper.Map(plan);

        foreach (CharacterAction action in actions)
        {
            _actionQueue.Enqueue(action);

            _logger.LogInformation(
                "Queued action: {Action}",
                action.Name);
        }
    }
}