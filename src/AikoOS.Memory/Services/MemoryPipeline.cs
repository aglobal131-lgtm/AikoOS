using AikoOS.Memory.Interfaces;
using AikoOS.Memory.Models;
using Microsoft.Extensions.Logging;

namespace AikoOS.Memory.Services;

public sealed class MemoryPipeline : IMemoryPipeline
{
    private readonly IMemoryService _memoryService;
    private readonly ILogger<MemoryPipeline> _logger;

    public MemoryPipeline(
        IMemoryService memoryService,
        ILogger<MemoryPipeline> logger)
    {
        _memoryService = memoryService;
        _logger = logger;
    }

    public async Task RecallAsync(
        MemoryContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        IReadOnlyList<MemoryEntry> memories =
            await _memoryService.GetAllAsync(
                cancellationToken);

        context.RelevantMemories =
            memories
                .Where(memory =>
                    !string.IsNullOrWhiteSpace(memory.Content))
                .Select(memory => memory.Content)
                .ToArray();

        _logger.LogInformation(
            "Recalled {MemoryCount} memory entries.",
            context.RelevantMemories.Count);
    }

    public async Task SaveAsync(
        MemoryContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        string content =
            BuildMemoryContent(context);

        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogDebug(
                "Memory save skipped because no content was available.");

            return;
        }

        await _memoryService.AddAsync(
            content,
            cancellationToken);

        _logger.LogInformation(
            "Saved a memory from the current interaction.");
    }

    private static string BuildMemoryContent(
        MemoryContext context)
    {
        string userInput =
            context.UserInput.Trim();

        string assistantResponse =
            context.AssistantResponse.Trim();

        if (string.IsNullOrWhiteSpace(userInput))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(assistantResponse))
        {
            return $"User: {userInput}";
        }

        return
            $"""
            User: {userInput}
            Assistant: {assistantResponse}
            """;
    }
}