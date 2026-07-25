namespace AikoOS.Behavior.Dispatchers;

public interface IActionDispatcher
{
    Task DispatchAsync(
        CancellationToken cancellationToken = default);
}