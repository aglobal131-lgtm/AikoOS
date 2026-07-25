using AikoOS.Core.Models;
using AikoOS.Memory.Repositories;
using AikoOS.Memory.Models;
using Microsoft.Extensions.Logging;

namespace AikoOS.Memory.Influence;

public sealed class MemoryInfluenceService : IMemoryInfluenceService

{
    private readonly IMemoryRepository _memoryRepository;
    private readonly ILogger<MemoryInfluenceService> _logger;

    public MemoryInfluenceService(
        IMemoryRepository memoryRepository,
        ILogger<MemoryInfluenceService> logger)
    {
        _memoryRepository = memoryRepository;
        _logger = logger;
    }
    public Task<double> GetAffinityAsync()
    {
        return Task.FromResult(0.5);
    }
    public async Task<AikoMood> CalculateMoodAsync()
    {
        IReadOnlyList<MemoryEntry> memories =
            await _memoryRepository.GetAllAsync();

        _logger.LogInformation(
            "Analyzing {Count} memories.",
            memories.Count);

        if (memories.Count == 0)
            return AikoMood.Neutral;

        if (memories.Count < 5)
            return AikoMood.Curious;

        if (memories.Count < 20)
            return AikoMood.Calm;

        return AikoMood.Happy;
    }

    public async Task<AikoEmotion> CalculateEmotionAsync()
    {
        AikoMood mood =
            await CalculateMoodAsync();

        return mood switch
        {
            AikoMood.Happy => AikoEmotion.Happy,

            AikoMood.Calm => AikoEmotion.Neutral,

            AikoMood.Curious => AikoEmotion.Thinking,

            _ => AikoEmotion.Neutral
        };
    }
}