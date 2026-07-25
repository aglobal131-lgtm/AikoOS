using AikoOS.Core.Models;

namespace AikoOS.Behavior.Personality;

public interface IPersonalityService
{
    AikoPersonality CurrentPersonality { get; }

    void SetPersonality(
        AikoPersonality personality);
}