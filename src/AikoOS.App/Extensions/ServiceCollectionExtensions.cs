using AikoOS.AI.DependencyInjection;
using AikoOS.App.Options;
using AikoOS.App.Services;
using AikoOS.App.ViewModels;
using AikoOS.App.Views;
using AikoOS.Core.Interfaces;
using AikoOS.App.ViewModels.Brain;
using AikoOS.Infrastructure.Repositories;
using AikoOS.Infrastructure.DependencyInjection;
using AikoOS.Memory.DependencyInjection;
using AikoOS.Runtime.DependencyInjection;
using AikoOS.Live2D.Services;
using AikoOS.Core.Services;
using AikoOS.App.Avatar;
using AikoOS.Behavior.Emotion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.App.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAikoOSInfrastructure();
        services.AddAikoOSAI(configuration);
        services.AddAikoOSMemory(configuration);
        services.AddAikoOSRuntime();
        services.AddTransient<ChatViewModel>();

        services.Configure<ApplicationOptions>(
            configuration.GetSection(
                ApplicationOptions.SectionName));

        services.AddSingleton<IChatRepository, PostgreSqlChatRepository>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<MemoryViewModel>();
        services.AddTransient<BrainInputViewModel>();

        services.AddSingleton<MainWindow>();

        services.AddSingleton<ILive2DService, Live2DService>();

        services.AddSingleton<AvatarStateBridge>();
        services.AddSingleton<BehaviorAvatarSyncService>();
        services.AddSingleton<IAvatarStateMapper, AvatarStateMapper>();

        services.AddSingleton<IEmotionEngine, EmotionEngine>();


        return services;
    }
}