using AikoOS.Core.Models;
using Microsoft.Extensions.Logging;

namespace AikoOS.Behavior.Mood;

public sealed class MoodManager : IMoodManager
{
    private readonly ILogger<MoodManager> _logger;

    private AikoMood _currentMood =
        AikoMood.Neutral;

    public MoodManager(
        ILogger<MoodManager> logger)
    {
        _logger = logger;
    }

    public AikoMood CurrentMood
        => _currentMood;

    public void SetMood(
        AikoMood mood)
    {
        if (_currentMood == mood)
            return;

        _currentMood = mood;

        _logger.LogInformation(
            "Mood changed to {Mood}.",
            mood);
    }
}