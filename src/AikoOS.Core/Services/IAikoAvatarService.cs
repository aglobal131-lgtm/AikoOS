namespace AikoOS.Core.Services;

/// <summary>
/// Abstraction dùng để điều khiển avatar của Aiko.
///
/// Interface này không phụ thuộc WPF, WebView2 hoặc Live2DControl,
/// vì vậy có thể được sử dụng trong các project net10.0.
/// </summary>
public interface IAikoAvatarService
{
    bool IsReady { get; }

    event EventHandler<bool>? ReadyChanged;

    Task<bool> WaitUntilReadyAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

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