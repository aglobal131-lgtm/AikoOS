using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AikoOS.Core.Voice;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace AikoOS.Infrastructure.Voice;

public sealed class ElevenLabsTtsService : ITtsService, IDisposable
{
    private const string DefaultBaseUrl = "https://api.elevenlabs.io";
    private const string DefaultModelId = "eleven_multilingual_v2";
    private const string OutputFormat = "mp3_44100_128";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ElevenLabsTtsService> _logger;
    private readonly SemaphoreSlim _speechLock = new(1, 1);
    private readonly object _playbackLock = new();

    private WaveOutEvent? _currentOutput;
    private bool _isDisposed;
    private volatile bool _isSpeaking;

    public ElevenLabsTtsService(
        IHttpClientFactory httpClientFactory,
        ILogger<ElevenLabsTtsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public bool IsSpeaking => _isSpeaking;

    public async Task SpeakAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        await _speechLock.WaitAsync(cancellationToken);

        try
        {
            await StopCurrentPlaybackAsync();

            string apiKey = GetRequiredEnvironmentVariable(
                "ELEVENLABS_API_KEY");

            string voiceId = GetRequiredEnvironmentVariable(
                "ELEVENLABS_VOICE_ID");

            byte[] audioData = await GenerateSpeechAsync(
                text.Trim(),
                apiKey,
                voiceId,
                cancellationToken);

            await PlayMp3Async(audioData, cancellationToken);
        }
        finally
        {
            _isSpeaking = false;
            _speechLock.Release();
        }
    }

    public Task StopAsync()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return StopCurrentPlaybackAsync();
    }

    private async Task<byte[]> GenerateSpeechAsync(
        string text,
        string apiKey,
        string voiceId,
        CancellationToken cancellationToken)
    {
        HttpClient client =
            _httpClientFactory.CreateClient("ElevenLabs");

        string requestUrl =
            $"/v1/text-to-speech/{Uri.EscapeDataString(voiceId)}" +
            $"?output_format={OutputFormat}";

        var requestBody = new ElevenLabsSpeechRequest
        {
            Text = text,
            ModelId = DefaultModelId,
            LanguageCode = "ja",
            VoiceSettings = new ElevenLabsVoiceSettings
            {
                Stability = 0.5,
                SimilarityBoost = 0.75,
                Style = 0.25,
                UseSpeakerBoost = true
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            requestUrl);

        request.Headers.Add("xi-api-key", apiKey);
        request.Content = JsonContent.Create(requestBody);

        _logger.LogInformation(
            "Requesting ElevenLabs speech generation. Characters: {CharacterCount}",
            text.Length);

        using HttpResponseMessage response =
            await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            _logger.LogError(
                "ElevenLabs request failed. Status: {StatusCode}. Response: {Response}",
                (int)response.StatusCode,
                errorContent);

            throw new HttpRequestException(
                $"ElevenLabs TTS failed with status " +
                $"{(int)response.StatusCode}: {errorContent}");
        }

        byte[] audioData =
            await response.Content.ReadAsByteArrayAsync(
                cancellationToken);

        if (audioData.Length == 0)
        {
            throw new InvalidOperationException(
                "ElevenLabs returned empty audio data.");
        }

        return audioData;
    }

    private async Task PlayMp3Async(
        byte[] audioData,
        CancellationToken cancellationToken)
    {
        using var audioStream = new MemoryStream(
            audioData,
            writable: false);

        using var mp3Reader = new Mp3FileReader(audioStream);
        using var output = new WaveOutEvent();

        var playbackFinished =
            new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

        output.PlaybackStopped += OnPlaybackStopped;
        output.Init(mp3Reader);

        lock (_playbackLock)
        {
            _currentOutput = output;
        }

        using CancellationTokenRegistration registration =
            cancellationToken.Register(
                static state =>
                {
                    if (state is WaveOutEvent waveOut)
                    {
                        waveOut.Stop();
                    }
                },
                output);

        try
        {
            _isSpeaking = true;

            _logger.LogInformation(
                "ElevenLabs audio playback started.");

            output.Play();

            await playbackFinished.Task.WaitAsync(
                cancellationToken);

            _logger.LogInformation(
                "ElevenLabs audio playback completed.");
        }
        finally
        {
            output.PlaybackStopped -= OnPlaybackStopped;

            lock (_playbackLock)
            {
                if (ReferenceEquals(_currentOutput, output))
                {
                    _currentOutput = null;
                }
            }

            _isSpeaking = false;
        }

        void OnPlaybackStopped(
            object? sender,
            StoppedEventArgs eventArgs)
        {
            if (eventArgs.Exception is not null)
            {
                playbackFinished.TrySetException(
                    eventArgs.Exception);
                return;
            }

            playbackFinished.TrySetResult(true);
        }
    }

    private Task StopCurrentPlaybackAsync()
    {
        WaveOutEvent? output;

        lock (_playbackLock)
        {
            output = _currentOutput;
        }

        if (output is null)
        {
            _isSpeaking = false;
            return Task.CompletedTask;
        }

        try
        {
            output.Stop();
        }
        catch (ObjectDisposedException)
        {
            // Playback đã kết thúc và WaveOutEvent đã được giải phóng.
        }

        _isSpeaking = false;
        return Task.CompletedTask;
    }

    private static string GetRequiredEnvironmentVariable(
        string variableName)
    {
        string? value =
            Environment.GetEnvironmentVariable(variableName);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Environment variable '{variableName}' is missing. " +
                "Add it to the AikoOS .env file.");
        }

        return value.Trim();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        lock (_playbackLock)
        {
            try
            {
                _currentOutput?.Stop();
            }
            catch (ObjectDisposedException)
            {
                // Đã được giải phóng.
            }

            _currentOutput = null;
        }

        _speechLock.Dispose();
    }

    private sealed class ElevenLabsSpeechRequest
    {
        [JsonPropertyName("text")]
        public required string Text { get; init; }

        [JsonPropertyName("model_id")]
        public required string ModelId { get; init; }

        [JsonPropertyName("language_code")]
        public string? LanguageCode { get; init; }

        [JsonPropertyName("voice_settings")]
        public ElevenLabsVoiceSettings? VoiceSettings { get; init; }
    }

    private sealed class ElevenLabsVoiceSettings
    {
        [JsonPropertyName("stability")]
        public double Stability { get; init; }

        [JsonPropertyName("similarity_boost")]
        public double SimilarityBoost { get; init; }

        [JsonPropertyName("style")]
        public double Style { get; init; }

        [JsonPropertyName("use_speaker_boost")]
        public bool UseSpeakerBoost { get; init; }
    }
}