namespace AikoOS.Behavior.Context;

public sealed class CharacterContext : ICharacterContext
{
    private readonly object _syncRoot = new();

    private CharacterRuntimeState _current = new();

    public event Action<CharacterRuntimeState>? StateChanged;

    public CharacterRuntimeState Current
    {
        get
        {
            lock (_syncRoot)
            {
                return _current;
            }
        }
    }

    public void SetState(string stateName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stateName);

        CharacterRuntimeState updatedState;

        lock (_syncRoot)
        {
            updatedState = new CharacterRuntimeState
            {
                StateName = stateName,
                EmotionName = _current.EmotionName,
                UpdatedAt = DateTime.UtcNow
            };

            _current = updatedState;
        }

        NotifyStateChanged(updatedState);
    }

    public void SetEmotion(string emotionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emotionName);

        CharacterRuntimeState updatedState;

        lock (_syncRoot)
        {
            updatedState = new CharacterRuntimeState
            {
                StateName = _current.StateName,
                EmotionName = emotionName,
                UpdatedAt = DateTime.UtcNow
            };

            _current = updatedState;
        }

        NotifyStateChanged(updatedState);
    }

    public void Update(
        string stateName,
        string emotionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stateName);
        ArgumentException.ThrowIfNullOrWhiteSpace(emotionName);

        CharacterRuntimeState updatedState;

        lock (_syncRoot)
        {
            updatedState = new CharacterRuntimeState
            {
                StateName = stateName,
                EmotionName = emotionName,
                UpdatedAt = DateTime.UtcNow
            };

            _current = updatedState;
        }

        NotifyStateChanged(updatedState);
    }

    private void NotifyStateChanged(
        CharacterRuntimeState state)
    {
        StateChanged?.Invoke(state);
    }
}