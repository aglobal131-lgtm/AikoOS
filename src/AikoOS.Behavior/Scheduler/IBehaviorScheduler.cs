namespace AikoOS.Behavior.Scheduler;

public interface IBehaviorScheduler
{
    Task StartAsync(
        CancellationToken cancellationToken = default);

    Task StopAsync(
        CancellationToken cancellationToken = default);
}