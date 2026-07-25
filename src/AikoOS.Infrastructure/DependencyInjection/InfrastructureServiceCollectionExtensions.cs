using AikoOS.Core.Interfaces;
using AikoOS.Core.Services;
using AikoOS.Infrastructure.Repositories;
using AikoOS.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<IUserSettingsService, JsonUserSettingsService>();
        services.AddScoped<IChatRepository, PostgreSqlChatRepository>();

        return services;
    }
}