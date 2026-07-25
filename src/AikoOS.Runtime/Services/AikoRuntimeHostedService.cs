using AikoOS.Runtime.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AikoOS.Runtime.Services;

public sealed class AikoRuntimeHostedService : IHostedService
{
    private readonly IRuntimeState _runtimeState;
    private readonly ILogger<AikoRuntimeHostedService> _logger;

    public AikoRuntimeHostedService(
        IRuntimeState runtimeState,
        ILogger<AikoRuntimeHostedService> logger)
    {
        _runtimeState = runtimeState;
        _logger = logger;
    }

    public Task StartAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "AikoOS Runtime is starting.");

        _runtimeState.SetStatus(
            RuntimeStatus.Ready);

        _logger.LogInformation(
            "AikoOS Runtime is ready.");

        return Task.CompletedTask;
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "AikoOS Runtime is stopping.");

        _runtimeState.SetStatus(
            RuntimeStatus.Stopping);

        _runtimeState.SetStatus(
            RuntimeStatus.Stopped);

        _logger.LogInformation(
            "AikoOS Runtime has stopped.");

        return Task.CompletedTask;
    }
}