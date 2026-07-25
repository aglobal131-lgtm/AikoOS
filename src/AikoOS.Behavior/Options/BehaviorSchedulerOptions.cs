namespace AikoOS.Behavior.Options;

public sealed class BehaviorSchedulerOptions
{
    public const string SectionName = "BehaviorScheduler";

    public int CheckIntervalSeconds { get; set; } = 5;

    public int IdleThresholdSeconds { get; set; } = 30;
}