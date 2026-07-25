using AikoOS.Live2D.Controls;

namespace AikoOS.Live2D.Services;

public interface ILive2DService
{
    bool IsAttached { get; }

    bool IsReady { get; }

    event EventHandler<bool>? ReadyChanged;

    Task<bool> WaitUntilReadyAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    void Attach(Live2DControl control);

    void Detach(Live2DControl control);

    Task<bool> PlayMotionAsync(
        string group,
        int index);

    Task<bool> SetExpressionAsync(
        string expressionName);

    Task<bool> LookAtAsync(
        double x,
        double y);

    Task<string[]> GetMotionGroupsAsync();

    Task<string[]> GetExpressionsAsync();
}