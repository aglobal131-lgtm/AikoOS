using AikoOS.Behavior.Actions;
using AikoOS.Behavior.Executors;
using AikoOS.Behavior.Queues;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Dispatchers;

public sealed class ActionDispatcher : IActionDispatcher
{
    private readonly IActionQueue _actionQueue;
    private readonly IReadOnlyList<IActionExecutor> _executors;
    private readonly ILogger<ActionDispatcher> _logger;

    public ActionDispatcher(
        IActionQueue actionQueue,
        IEnumerable<IActionExecutor> executors,
        ILogger<ActionDispatcher> logger)
    {
        ArgumentNullException.ThrowIfNull(actionQueue);
        ArgumentNullException.ThrowIfNull(executors);
        ArgumentNullException.ThrowIfNull(logger);

        _actionQueue = actionQueue;
        _executors = executors.ToArray();
        _logger = logger;
    }

    public async Task DispatchAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (_actionQueue.TryDequeue(
                   out CharacterAction? action))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (action is null)
            {
                _logger.LogWarning(
                    "Behavior action queue returned a null action.");

                continue;
            }

            IActionExecutor? executor =
                _executors.FirstOrDefault(
                    candidate =>
                        candidate.CanExecute(action));

            if (executor is null)
            {
                _logger.LogWarning(
                    "No executor found for behavior action {ActionName}.",
                    action.Name);

                continue;
            }

            try
            {
                await executor.ExecuteAsync(
                        action,
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
                    "Executor {ExecutorName} failed for behavior action {ActionName}.",
                    executor.GetType().Name,
                    action.Name);
            }
        }
    }
}