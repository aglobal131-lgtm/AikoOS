using AikoOS.Memory.Models;

namespace AikoOS.Memory.Interfaces;

public interface IMemoryPipeline
{
    Task RecallAsync(
        MemoryContext context,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        MemoryContext context,
        CancellationToken cancellationToken = default);
}