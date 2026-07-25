namespace AikoOS.Behavior.State;

using AikoOS.Behavior.Actions;

public sealed class CharacterState
{
    public string CurrentAction { get; set; } = CharacterActionNames.Idle;

    public string Emotion { get; set; } = "neutral";

    public bool IsTalking { get; set; }

    public bool IsMoving { get; set; }

    public bool IsSleeping { get; set; }
}