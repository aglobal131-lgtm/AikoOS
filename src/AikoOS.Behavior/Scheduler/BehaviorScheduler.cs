using AikoOS.Behavior.Dispatchers;
using AikoOS.Behavior.EventSources;
using AikoOS.Behavior.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AikoOS.Behavior.Scheduler;

public sealed class BehaviorScheduler : IBehaviorScheduler
{
    private readonly IEnumerable<IBehaviorEventSource> _eventSources;
    private readonly IActionDispatcher _actionDispatcher;
    private readonly BehaviorSchedulerOptions _options;
    private readonly ILogger<BehaviorScheduler> _logger;

    private CancellationTokenSource? _schedulerCancellation;
    private Task? _schedulerTask;

    public BehaviorScheduler(
        IEnumerable<IBehaviorEventSource> eventSources,
        IActionDispatcher actionDispatcher,
        IOptions<BehaviorSchedulerOptions> options,
        ILogger<BehaviorScheduler> logger)
    {
        _eventSources = eventSources;
        _actionDispatcher = actionDispatcher;
        _options = options.Value;
        _logger = logger;
    }

    public Task StartAsync(
        CancellationToken cancellationToken = default)
    {
        if (_schedulerTask is not null)
        {
            return Task.CompletedTask;
        }

        _schedulerCancellation =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        _schedulerTask = Task.Run(
            () => RunAsync(_schedulerCancellation.Token),
            CancellationToken.None);

        _logger.LogInformation(
            "Behavior Scheduler started with {EventSourceCount} event source(s).",
            _eventSources.Count());

        return Task.CompletedTask;
    }

    public async Task StopAsync(
        CancellationToken cancellationToken = default)
    {
        CancellationTokenSource? cancellation =
            _schedulerCancellation;

        Task? schedulerTask =
            _schedulerTask;

        if (cancellation is null ||
            schedulerTask is null)
        {
            return;
        }

        _logger.LogInformation(
            "Behavior Scheduler is stopping.");

        cancellation.Cancel();

        try
        {
            await schedulerTask
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Bình thường khi Scheduler hoặc ứng dụng đang dừng.
        }
        finally
        {
            cancellation.Dispose();

            _schedulerCancellation = null;
            _schedulerTask = null;
        }

        _logger.LogInformation(
            "Behavior Scheduler stopped.");
    }

    private async Task RunAsync(
        CancellationToken cancellationToken)
    {
        TimeSpan checkInterval =
            TimeSpan.FromSeconds(
                _options.CheckIntervalSeconds);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (IBehaviorEventSource eventSource
                         in _eventSources)
                {
                    cancellationToken
                        .ThrowIfCancellationRequested();

                    try
                    {
                        await eventSource.UpdateAsync(
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                        when (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(
                            exception,
                            "Behavior event source {EventSourceName} failed.",
                            eventSource.GetType().Name);
                    }
                }

                try
                {
                    await _actionDispatcher.DispatchAsync(
                            cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Behavior action dispatcher failed.");
                }

                await Task.Delay(
                        checkInterval,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Behavior Scheduler loop cancelled.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Behavior Scheduler loop failed.");
        }
    }
}