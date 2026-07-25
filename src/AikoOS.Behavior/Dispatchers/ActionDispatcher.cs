using AikoOS.Behavior.Queues;
using AikoOS.Behavior.Executors;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Dispatchers;

public sealed class ActionDispatcher : IActionDispatcher
{
    private readonly IActionQueue _actionQueue;

    private readonly IEnumerable<IActionExecutor> _executors;
    private readonly ILogger<ActionDispatcher> _logger;

    public ActionDispatcher(
    IActionQueue actionQueue,
    IEnumerable<IActionExecutor> executors,
    ILogger<ActionDispatcher> logger)
    {
        _actionQueue = actionQueue;
        _executors = executors;
        _logger = logger;
    }

    public async Task DispatchAsync(
    CancellationToken cancellationToken = default)
    {
        while (_actionQueue.TryDequeue(out var action))
        {
            IActionExecutor? executor =
                _executors.FirstOrDefault(
                    x => x.CanExecute(action!));

            if (executor is null)
            {
                _logger.LogWarning(
                    "No executor found for action {Action}",
                    action!.Name);

                continue;
            }

            await executor.ExecuteAsync(
                action!,
                cancellationToken);
        }
    }
}