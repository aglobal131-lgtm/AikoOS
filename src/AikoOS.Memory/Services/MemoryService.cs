using AikoOS.Memory.Models;
using AikoOS.Memory.Repositories;
using Microsoft.Extensions.Logging;

namespace AikoOS.Memory.Services;

public sealed class MemoryService : IMemoryService
{
    private readonly IMemoryRepository _repository;
    private readonly ILogger<MemoryService> _logger;

    public MemoryService(
        IMemoryRepository repository,
        ILogger<MemoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<IReadOnlyList<MemoryEntry>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(cancellationToken);
    }

    public async Task<MemoryEntry> AddAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        string normalized = content.Trim();

        MemoryEntry memory =
            await _repository.AddAsync(
                normalized,
                cancellationToken);

        _logger.LogInformation(
            "Memory {MemoryId} added.",
            memory.Id);

        return memory;
    }

    public Task DeleteAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return _repository.DeleteAsync(
            id,
            cancellationToken);
    }
}