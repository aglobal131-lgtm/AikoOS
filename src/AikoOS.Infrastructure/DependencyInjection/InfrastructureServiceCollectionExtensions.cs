using AikoOS.Core.Interfaces;
using AikoOS.Core.Services;
using AikoOS.Infrastructure.Repositories;
using AikoOS.Infrastructure.Settings;
using AikoOS.Core.Voice;
using AikoOS.Infrastructure.Voice;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IUserSettingsService, JsonUserSettingsService>();
        services.AddScoped<IChatRepository, PostgreSqlChatRepository>();

        services.AddHttpClient(
    "ElevenLabs",
    client =>
    {
        client.BaseAddress = new Uri(
            "https://api.elevenlabs.io");

        client.Timeout = TimeSpan.FromSeconds(60);
    });

        services.AddSingleton<
            ITtsService,
            ElevenLabsTtsService>();

        services.AddSingleton<ITtsService, ElevenLabsTtsService>();

        return services;
    }
}