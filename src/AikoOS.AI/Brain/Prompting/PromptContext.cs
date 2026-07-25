using AikoOS.AI.Brain.Planning;

namespace AikoOS.AI.Brain.Prompting;

public sealed class PromptContext
{
    public required PlanningContext PlanningContext { get; init; }

    public string UserName { get; init; } = "User";

    public string AssistantName { get; init; } = "Aiko";
}