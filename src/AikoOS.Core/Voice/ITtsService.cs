namespace AikoOS.Core.Voice;

public interface ITtsService
{
    bool IsSpeaking { get; }

    Task SpeakAsync(
        string text,
        CancellationToken cancellationToken = default);

    Task StopAsync();
}