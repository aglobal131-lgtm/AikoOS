using AikoOS.AI.Brain.Decisions;

namespace AikoOS.AI.Brain.Planning;

public interface IPlanningService
{
    Task<Plan> CreatePlanAsync(
    PlanningContext context,
    CancellationToken cancellationToken = default);
}