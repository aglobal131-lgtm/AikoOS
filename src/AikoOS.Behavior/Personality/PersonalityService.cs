using AikoOS.Core.Models;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Personality;

public sealed class PersonalityService
    : IPersonalityService
{
    private readonly ILogger<PersonalityService> _logger;

    private AikoPersonality _current =
        AikoPersonality.Gentle;

    public PersonalityService(
        ILogger<PersonalityService> logger)
    {
        _logger = logger;
    }

    public AikoPersonality CurrentPersonality
        => _current;

    public void SetPersonality(
        AikoPersonality personality)
    {
        if (_current == personality)
            return;

        _current = personality;

        _logger.LogInformation(
            "Personality changed to {Personality}.",
            personality);
    }
}