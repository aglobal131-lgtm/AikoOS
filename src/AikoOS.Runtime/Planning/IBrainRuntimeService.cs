using AikoOS.AI.Brain.Decisions;
using AikoOS.Runtime.Brain.Models;

namespace AikoOS.Runtime.Planning;

public interface IBrainRuntimeService
{
    Task<Plan> ExecuteAsync(
        BrainRequest request,
        CancellationToken cancellationToken = default);
}