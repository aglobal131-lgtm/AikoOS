using System.Windows;
using System.Windows.Threading;
using AikoOS.Core.Models;
using AikoOS.Core.Services;
using AikoOS.Live2D.Services;
using Microsoft.Extensions.Logging;

namespace AikoOS.App.Avatar;

public sealed class AvatarStateBridge : IDisposable
{
    private readonly IAikoStateService _stateService;
    private readonly ILive2DService _live2DService;
    private readonly ILogger<AvatarStateBridge> _logger;

    private readonly SemaphoreSlim _syncLock =
        new(1, 1);

    private bool _isDisposed;

    public AvatarStateBridge(
        IAikoStateService stateService,
        ILive2DService live2DService,
        ILogger<AvatarStateBridge> logger)
    {
        _stateService = stateService;
        _live2DService = live2DService;
        _logger = logger;

        _stateService.StateChanged +=
            StateService_StateChanged;

        _live2DService.ReadyChanged +=
            Live2DService_ReadyChanged;

        _logger.LogInformation(
            "AvatarStateBridge was created.");
    }

    private async void StateService_StateChanged(
        object? sender,
        AikoAvatarState state)
    {
        try
        {
            await ApplyStateAsync(state);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to apply avatar state.");
        }
    }

    private async void Live2DService_ReadyChanged(
        object? sender,
        bool isReady)
    {
        if (!isReady)
        {
            return;
        }

        try
        {
            await ApplyStateAsync(
                _stateService.CurrentState);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to apply current state after Live2D became ready.");
        }
    }

    private async Task ApplyStateAsync(
    AikoAvatarState state)
    {
        Dispatcher? dispatcher =
            Application.Current?.Dispatcher;

        if (dispatcher is not null &&
            !dispatcher.CheckAccess())
        {
            await dispatcher
                .InvokeAsync(
                    () => ApplyStateAsync(state))
                .Task
                .Unwrap();

            return;
        }

        if (!_live2DService.IsReady)
        {
            _logger.LogDebug(
                "Live2D is not ready. Avatar state will be applied later.");

            return;
        }

        await _syncLock.WaitAsync();

        try
        {
            if (!string.IsNullOrWhiteSpace(
                    state.ExpressionName))
            {
                bool expressionResult =
                    await _live2DService.SetExpressionAsync(
                        state.ExpressionName);

                _logger.LogDebug(
                    "Expression {ExpressionName} result: {Result}.",
                    state.ExpressionName,
                    expressionResult);
            }

            if (!string.IsNullOrWhiteSpace(
                    state.MotionGroup))
            {
                bool motionResult =
                    await _live2DService.PlayMotionAsync(
                        state.MotionGroup,
                        state.MotionIndex);

                _logger.LogDebug(
                    "Motion {MotionGroup}/{MotionIndex} result: {Result}.",
                    state.MotionGroup,
                    state.MotionIndex,
                    motionResult);
            }

            bool lookResult =
                await _live2DService.LookAtAsync(
                    state.LookX,
                    state.LookY);

            _logger.LogInformation(
                "Avatar state synchronized. Emotion: {Emotion}, LookResult: {LookResult}.",
                state.Emotion,
                lookResult);
        }
        finally
        {
            _syncLock.Release();
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _stateService.StateChanged -=
            StateService_StateChanged;

        _live2DService.ReadyChanged -=
            Live2DService_ReadyChanged;

        _syncLock.Dispose();

        _isDisposed = true;

        _logger.LogInformation(
            "AvatarStateBridge was disposed.");
    }
}