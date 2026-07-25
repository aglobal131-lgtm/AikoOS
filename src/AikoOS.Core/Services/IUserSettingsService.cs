using AikoOS.Core.Models;

namespace AikoOS.Core.Services;

public interface IUserSettingsService
{
    Task<UserSettings> LoadAsync(
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        UserSettings settings,
        CancellationToken cancellationToken = default);
}