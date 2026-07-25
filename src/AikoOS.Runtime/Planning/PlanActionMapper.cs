using AikoOS.AI.Brain.Decisions;
using AikoOS.Behavior.Actions;

namespace AikoOS.Runtime.Planning;

public sealed class PlanActionMapper
    : IPlanActionMapper
{
    public IReadOnlyList<CharacterAction> Map(
        Plan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        List<CharacterAction> actions = [];

        foreach (Decision decision in plan.Decisions)
        {
            if (string.IsNullOrWhiteSpace(
                    decision.Action))
            {
                continue;
            }

            CharacterAction action =
                new()
                {
                    Name = decision.Action,
                    Emotion = decision.Emotion,
                    Speech = decision.Speech,
                    Parameters =
                        new Dictionary<string, object?>(
                            decision.Parameters)
                };

            actions.Add(action);
        }

        return actions;
    }
}