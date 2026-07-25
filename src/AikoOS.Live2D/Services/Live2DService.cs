using AikoOS.Live2D.Controls;

namespace AikoOS.Live2D.Services;

public sealed class Live2DService : ILive2DService
{
    private Live2DControl? _control;

    public bool IsAttached =>
        _control is not null;

    public bool IsReady =>
        _control?.IsReady == true;

    public event EventHandler<bool>? ReadyChanged;

    /// <summary>
    /// Chờ đến khi Live2DControl và JavaScript bridge sẵn sàng.
    /// </summary>
    public async Task<bool> WaitUntilReadyAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        if (IsReady)
        {
            return true;
        }

        if (timeout <= TimeSpan.Zero)
        {
            return false;
        }

        TaskCompletionSource<bool> completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnReadyChanged(
            object? sender,
            bool isReady)
        {
            if (isReady)
            {
                completionSource.TrySetResult(true);
            }
        }

        ReadyChanged += OnReadyChanged;

        try
        {
            // Tránh trường hợp model chuyển sang Ready ngay
            // trước khi đăng ký sự kiện hoàn tất.
            if (IsReady)
            {
                return true;
            }

            using CancellationTokenSource timeoutSource =
                new(timeout);

            using CancellationTokenSource linkedSource =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutSource.Token);

            try
            {
                await completionSource.Task.WaitAsync(
                    linkedSource.Token);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
        finally
        {
            ReadyChanged -= OnReadyChanged;
        }
    }
    public void Attach(Live2DControl control)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (ReferenceEquals(_control, control))
        {
            return;
        }

        bool previousReadyState = IsReady;

        if (_control is not null)
        {
            _control.ReadyChanged -=
                Control_ReadyChanged;
        }

        _control = control;

        _control.ReadyChanged +=
            Control_ReadyChanged;

        if (previousReadyState != IsReady)
        {
            ReadyChanged?.Invoke(
                this,
                IsReady);
        }
    }

    /// <summary>
    /// Chỉ gỡ control nếu đúng là control đang được gắn.
    /// </summary>
    public void Detach(Live2DControl control)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (!ReferenceEquals(_control, control))
        {
            return;
        }

        bool wasReady = IsReady;

        _control.ReadyChanged -=
            Control_ReadyChanged;

        _control = null;

        if (wasReady)
        {
            ReadyChanged?.Invoke(
                this,
                false);
        }
    }

    /// <summary>
    /// Chuyển tiếp sự kiện ReadyChanged từ control ra service.
    /// </summary>
    private void Control_ReadyChanged(
        object? sender,
        bool isReady)
    {
        ReadyChanged?.Invoke(
            this,
            isReady);
    }

    public async Task<bool> PlayMotionAsync(
        string group,
        int index)
    {
        Live2DControl? control = _control;

        if (control is null ||
            !control.IsReady)
        {
            return false;
        }

        return await control.PlayMotionAsync(
            group,
            index);
    }

    public async Task<bool> SetExpressionAsync(
        string expressionName)
    {
        Live2DControl? control = _control;

        if (control is null ||
            !control.IsReady)
        {
            return false;
        }

        return await control.SetExpressionAsync(
            expressionName);
    }

    public async Task<bool> LookAtAsync(
        double x,
        double y)
    {
        Live2DControl? control = _control;

        if (control is null ||
            !control.IsReady)
        {
            return false;
        }

        return await control.LookAtAsync(
            x,
            y);
    }

    public async Task<string[]> GetMotionGroupsAsync()
    {
        Live2DControl? control = _control;

        if (control is null ||
            !control.IsReady)
        {
            return [];
        }

        return await control.GetMotionGroupsAsync();
    }

    public async Task<string[]> GetExpressionsAsync()
    {
        Live2DControl? control = _control;

        if (control is null ||
            !control.IsReady)
        {
            return [];
        }

        return await control.GetExpressionsAsync();
    }
}