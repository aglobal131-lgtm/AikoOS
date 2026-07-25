using AikoOS.Core.Services;
using AikoOS.Infrastructure.Settings;
using AikoOS.Infrastructure.Communication;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IUserSettingsService, JsonUserSettingsService>();

        services.AddSingleton<IUnityTransport, NamedPipeUnityTransport>();
        services.AddSingleton<UnityCommandService>();

        return services;
    }
}