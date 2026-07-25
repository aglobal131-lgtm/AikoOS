using System.Text.Json;
using AikoOS.Core.Models;
using AikoOS.Core.Services;
using Microsoft.Extensions.Logging;

namespace AikoOS.Infrastructure.Settings;

public sealed class JsonUserSettingsService
    : IUserSettingsService
{
    private readonly ILogger<JsonUserSettingsService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _settingsFilePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public JsonUserSettingsService(
        ILogger<JsonUserSettingsService> logger)
    {
        _logger = logger;

        string applicationDirectory = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "AikoOS");

        Directory.CreateDirectory(applicationDirectory);

        _settingsFilePath = Path.Combine(
            applicationDirectory,
            "settings.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<UserSettings> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken);

        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                _logger.LogInformation(
                    "Settings file does not exist. Default settings will be used.");

                return new UserSettings();
            }

            await using FileStream stream =
                File.OpenRead(_settingsFilePath);

            UserSettings? settings =
                await JsonSerializer.DeserializeAsync<UserSettings>(
                    stream,
                    _jsonOptions,
                    cancellationToken);

            if (settings is null)
            {
                _logger.LogWarning(
                    "Settings file contained no valid settings. Defaults will be used.");

                return new UserSettings();
            }

            _logger.LogInformation(
                "User settings loaded from {SettingsFilePath}.",
                _settingsFilePath);

            return settings;
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "Settings file contains invalid JSON. Defaults will be used.");

            return new UserSettings();
        }
        catch (IOException exception)
        {
            _logger.LogError(
                exception,
                "Could not read the settings file. Defaults will be used.");

            return new UserSettings();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task SaveAsync(
        UserSettings settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        await _fileLock.WaitAsync(cancellationToken);

        try
        {
            string temporaryFilePath =
                _settingsFilePath + ".tmp";

            await using (FileStream stream = File.Create(
                temporaryFilePath))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    settings,
                    _jsonOptions,
                    cancellationToken);
            }

            File.Move(
                temporaryFilePath,
                _settingsFilePath,
                overwrite: true);

            _logger.LogInformation(
                "User settings saved to {SettingsFilePath}.",
                _settingsFilePath);
        }
        catch (IOException exception)
        {
            _logger.LogError(
                exception,
                "Could not save user settings.");

            throw;
        }
        finally
        {
            _fileLock.Release();
        }
    }
}