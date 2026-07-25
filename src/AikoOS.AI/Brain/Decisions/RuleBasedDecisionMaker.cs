using AikoOS.AI.Brain.Planning;

namespace AikoOS.AI.Brain.Decisions;

public sealed class RuleBasedDecisionMaker
    : IDecisionMaker
{
    public Task<Plan> DecideAsync(
        PlanningContext context,
        CancellationToken cancellationToken = default)
    {
        Plan plan = new();

        if (context.CurrentState == "Sleeping")
        {
            plan.Decisions.Add(
                new Decision
                {
                    Action = "wake_up",
                    Emotion = "Neutral",
                    Speech = string.Empty

                });

            return Task.FromResult(plan);
        }

        plan.Decisions.Add(
            new Decision
            {
                Action = "idle",
                Emotion = "Neutral",
                Speech = string.Empty

            });

        return Task.FromResult(plan);
    }
}