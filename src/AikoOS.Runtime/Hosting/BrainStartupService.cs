using AikoOS.Runtime.Brain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AikoOS.Runtime.Hosting;

public sealed class BrainStartupService : IHostedService
{
    private readonly IBrainRequestService _brainRequestService;
    private readonly ILogger<BrainStartupService> _logger;

    public BrainStartupService(
        IBrainRequestService brainRequestService,
        ILogger<BrainStartupService> logger)
    {
        _brainRequestService = brainRequestService;
        _logger = logger;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting temporary Brain request test...");

        await _brainRequestService.ProcessAsync(
            "Aiko",
            cancellationToken);
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}