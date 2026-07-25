using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.State;

namespace AikoOS.Behavior.Context;

public sealed class CharacterRuntimeState
{
    public string StateName { get; init; } =
        CharacterStateNames.Idle;

    public string EmotionName { get; init; } =
        EmotionNames.Neutral;

    public DateTime UpdatedAt { get; init; } =
        DateTime.UtcNow;
}