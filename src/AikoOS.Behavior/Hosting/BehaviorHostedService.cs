using AikoOS.Behavior.Bus;
using AikoOS.Behavior.Events;
using AikoOS.Behavior.Scheduler;
using AikoOS.Behavior.Brain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Hosting;

public sealed class BehaviorHostedService : BackgroundService
{
    private readonly IBehaviorEventBus _eventBus;
    private readonly IBehaviorScheduler _scheduler;
    private readonly ILogger<BehaviorHostedService> _logger;
    private readonly IBehaviorEngine _behaviorEngine;
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

        PublishStartupEvent();

        await _scheduler.StartAsync(stoppingToken);
    }

    private void PublishStartupEvent()
    {
        _logger.LogInformation(
            "Publishing startup behavior event.");

        _eventBus.Publish(
            new BehaviorEvent
            {
                Name = BehaviorEventNames.Startup
            });
    }

    private void SubscribeToBehaviorEvents()
    {
        Subscribe(BehaviorEventNames.Startup);
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
                HandleBehaviorEvent);

        _subscriptions.Add(subscription);
    }

    private void HandleBehaviorEvent(
    BehaviorEvent behaviorEvent)
    {
        try
        {
            _behaviorEngine
                .HandleEventAsync(behaviorEvent)
                .GetAwaiter()
                .GetResult();
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