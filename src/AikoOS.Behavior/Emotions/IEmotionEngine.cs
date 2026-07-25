using AikoOS.Core.Models;

namespace AikoOS.Behavior.Emotion;

public interface IEmotionEngine
{
    void SetEmotion(AikoEmotion emotion);

    AikoEmotion CurrentEmotion { get; }
}