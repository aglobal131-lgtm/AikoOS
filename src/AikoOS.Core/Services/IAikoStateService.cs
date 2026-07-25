using AikoOS.Core.Models;

namespace AikoOS.Core.Services;

public interface IAikoStateService
{
    AikoAvatarState CurrentState { get; }

    event EventHandler<AikoAvatarState>? StateChanged;

    void SetState(AikoAvatarState state);

    void SetEmotion(AikoEmotion emotion);

    void Reset();
}