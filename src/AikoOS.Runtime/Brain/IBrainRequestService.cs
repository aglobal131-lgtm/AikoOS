using AikoOS.Runtime.Brain.Models;

namespace AikoOS.Runtime.Brain;

public interface IBrainRequestService
{
    Task<BrainResponse> ProcessAsync(
        string userInput,
        CancellationToken cancellationToken = default);

    Task<BrainResponse> ProcessAsync(
        BrainRequest request,
        CancellationToken cancellationToken = default);
}