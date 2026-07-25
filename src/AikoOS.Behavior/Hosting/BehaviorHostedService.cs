using AikoOS.Behavior.Brain;
using AikoOS.Behavior.Bus;
using AikoOS.Behavior.Events;
using AikoOS.Behavior.Scheduler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Hosting;

public sealed class BehaviorHostedService : BackgroundService
{
    private readonly IBehaviorEventBus _eventBus;
    private readonly IBehaviorEngine _behaviorEngine;
    private readonly IBehaviorScheduler _scheduler;
    private readonly ILogger<BehaviorHostedService> _logger;

    private readonly List<IDisposable> _subscriptions = new();

    public BehaviorHostedService(
        IBehaviorEventBus eventBus,
        IBehaviorEngine behaviorEngine,
        IBehaviorScheduler scheduler,
        ILogger<BehaviorHostedService> logger)
    {
        _eventBus = eventBus;
        _behaviorEngine = behaviorEngine;
        _scheduler = scheduler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Behavior hosted service is starting.");

        SubscribeToBehaviorEvents();

        await PublishStartupEventAsync(stoppingToken);

        await _scheduler.StartAsync(stoppingToken);
    }

    private async Task PublishStartupEventAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing startup behavior event.");

        BehaviorEvent startupEvent = new()
        {
            Name = BehaviorEventNames.Startup
        };

        await HandleBehaviorEventAsync(
            startupEvent,
            cancellationToken);
    }

    private void SubscribeToBehaviorEvents()
    {
        Subscribe(BehaviorEventNames.UserIdle);
        Subscribe(BehaviorEventNames.UserActive);

        _logger.LogInformation(
            "Behavior engine subscribed to behavior events.");
    }

    private void Subscribe(string eventName)
    {
        IDisposable subscription =
            _eventBus.Subscribe(
                eventName,
                behaviorEvent =>
                {
                    _ = HandleBehaviorEventAsync(
                        behaviorEvent,
                        CancellationToken.None);
                });

        _subscriptions.Add(subscription);
    }

    private async Task HandleBehaviorEventAsync(
        BehaviorEvent behaviorEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            await _behaviorEngine.HandleEventAsync(
                behaviorEvent,
                cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Behavior event handling was cancelled.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to handle behavior event {EventName}.",
                behaviorEvent.Name);
        }
    }

    public override void Dispose()
    {
        foreach (IDisposable subscription in _subscriptions)
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();

        base.Dispose();
    }
}