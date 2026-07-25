using AikoOS.AI.Brain.Decisions;
using AikoOS.Behavior.Actions;

namespace AikoOS.Runtime.Planning;

public interface IPlanActionMapper
{
    IReadOnlyList<CharacterAction> Map(
        Plan plan);
}