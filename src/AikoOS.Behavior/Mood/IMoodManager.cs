using AikoOS.Core.Models;

namespace AikoOS.Behavior.Mood;

public interface IMoodManager
{
    AikoMood CurrentMood { get; }

    void SetMood(
        AikoMood mood);
}