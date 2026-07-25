using AikoOS.AI.Brain.Decisions;

namespace AikoOS.AI.Brain.Planning;

public sealed class PlanningService
    : IPlanningService
{
    private readonly IDecisionMaker _decisionMaker;

    public PlanningService(
        IDecisionMaker decisionMaker)
    {
        _decisionMaker = decisionMaker;
    }

    public async Task<Plan> CreatePlanAsync(
    PlanningContext context,
    CancellationToken cancellationToken = default)
    {
        // Hiện tại context chưa được sử dụng.
        // Bước sau sẽ truyền vào DecisionMaker.

        return await _decisionMaker.DecideAsync(context, cancellationToken);
    }
}