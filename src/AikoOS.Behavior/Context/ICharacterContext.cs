namespace AikoOS.Behavior.Context;

public interface ICharacterContext
{
    CharacterRuntimeState Current { get; }

    event Action<CharacterRuntimeState>? StateChanged;

    void SetState(string stateName);

    void SetEmotion(string emotionName);

    void Update(
        string stateName,
        string emotionName);
}