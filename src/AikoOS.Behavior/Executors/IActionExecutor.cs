using AikoOS.Behavior.Actions;

namespace AikoOS.Behavior.Executors;

public interface IActionExecutor
{
    bool CanExecute(
        CharacterAction action);

    Task ExecuteAsync(
        CharacterAction action,
        CancellationToken cancellationToken = default);
}