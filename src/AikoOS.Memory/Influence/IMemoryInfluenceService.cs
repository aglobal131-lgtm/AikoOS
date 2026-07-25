using AikoOS.Core.Models;

namespace AikoOS.Memory.Influence;

public interface IMemoryInfluenceService
{
    Task<AikoMood> CalculateMoodAsync();

    Task<AikoEmotion> CalculateEmotionAsync();

    Task<double> GetAffinityAsync();
}