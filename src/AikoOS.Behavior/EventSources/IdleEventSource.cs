using AikoOS.Behavior.Bus;
using AikoOS.Behavior.Events;
using AikoOS.Behavior.Idle;
using AikoOS.Behavior.Options;
using AikoOS.Behavior.Context;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AikoOS.Behavior.EventSources;

public sealed class IdleEventSource : IBehaviorEventSource
{
    private readonly IBehaviorEventBus _eventBus;
    private readonly IIdleMonitor _idleMonitor;
    private readonly BehaviorSchedulerOptions _options;
    private readonly ILogger<IdleEventSource> _logger;
    private readonly IBehaviorContext _context;

    private bool _idleEventPublished;

    public IdleEventSource(
    IBehaviorEventBus eventBus,
    IIdleMonitor idleMonitor,
    IBehaviorContext context,
    IOptions<BehaviorSchedulerOptions> options,
    ILogger<IdleEventSource> logger)
    {
        _eventBus = eventBus;
        _idleMonitor = idleMonitor;
        _context = context;
        _options = options.Value;
        _logger = logger;
    }

    public Task UpdateAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        TimeSpan idleDuration =
            _idleMonitor.GetCurrentIdleDuration();

        _context.UpdateIdleDuration(idleDuration);

        TimeSpan idleThreshold =
            TimeSpan.FromSeconds(
                _options.IdleThresholdSeconds);

        _logger.LogDebug(
            "Current user idle duration: {IdleDuration}.",
            idleDuration);

        if (idleDuration >= idleThreshold)
        {
            PublishIdleEventIfNeeded(
                idleDuration);

            return Task.CompletedTask;
        }

        ResetIdleStateIfNeeded();

        return Task.CompletedTask;
    }

    private void PublishIdleEventIfNeeded(
        TimeSpan idleDuration)
    {
        if (_idleEventPublished)
        {
            return;
        }

        _idleEventPublished = true;

        _logger.LogInformation(
            "User has been idle for {IdleDuration}. Publishing UserIdle event.",
            idleDuration);

        _eventBus.Publish(
            new BehaviorEvent
            {
                Name = BehaviorEventNames.UserIdle
            });

        _idleEventPublished = true;
    }

    private void ResetIdleStateIfNeeded()
    {
        if (!_idleEventPublished)
        {
            return;
        }

        _idleEventPublished = false;

        _logger.LogInformation(
            "User activity detected. Publishing UserActive event.");

        _eventBus.Publish(
            new BehaviorEvent
            {
                Name = BehaviorEventNames.UserActive
            });

        _idleEventPublished = false;
    }
}