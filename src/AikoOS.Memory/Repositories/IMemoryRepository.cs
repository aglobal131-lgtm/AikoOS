using AikoOS.Memory.Models;

namespace AikoOS.Memory.Repositories;

public interface IMemoryRepository
{
    Task<IReadOnlyList<MemoryEntry>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<MemoryEntry> AddAsync(
        string content,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long id,
        CancellationToken cancellationToken = default);
}