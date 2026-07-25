using AikoOS.AI.Brain.Planning;

namespace AikoOS.AI.Brain.Decisions;

public interface IDecisionMaker
{
    Task<Plan> DecideAsync(
        PlanningContext context,
        CancellationToken cancellationToken = default);
}