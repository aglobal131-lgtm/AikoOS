namespace AikoOS.Core.Models;

public sealed record AikoAvatarState
{
    public AikoEmotion Emotion { get; init; } = AikoEmotion.Neutral;

    public AikoMood Mood { get; init; } = AikoMood.Neutral;
    public AikoPersonality Personality { get; init; }
    = AikoPersonality.Gentle;

    public string? MotionGroup { get; init; }

    public int MotionIndex { get; init; }

    public string? ExpressionName { get; init; }

    public double LookX { get; init; }

    public double LookY { get; init; }

    public DateTimeOffset UpdatedAt { get; init; } =
        DateTimeOffset.UtcNow;
}