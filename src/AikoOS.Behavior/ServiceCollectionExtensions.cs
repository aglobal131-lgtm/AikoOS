using AikoOS.Behavior.Brain;
using AikoOS.Behavior.Bus;
using AikoOS.Behavior.Context;
using AikoOS.Behavior.Debug;
using AikoOS.Behavior.Dispatchers;
using AikoOS.Behavior.EventSources;
using AikoOS.Behavior.Executors;
using AikoOS.Behavior.Hosting;
using AikoOS.Behavior.Idle;
using AikoOS.Behavior.Mood;
using AikoOS.Behavior.Options;
using AikoOS.Behavior.Personality;
using AikoOS.Behavior.Queues;
using AikoOS.Behavior.Rules;
using AikoOS.Behavior.Scheduler;
using AikoOS.Behavior.Services;
using AikoOS.Behavior.State;
using AikoOS.Behavior.Emotion;
using AikoOS.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.Behavior;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBehavior(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BehaviorSchedulerOptions>(
            configuration.GetSection(
                BehaviorSchedulerOptions.SectionName));

        services.AddSingleton<IActionQueue, ActionQueue>();

        services.AddSingleton<
            IBehaviorEventBus,
            BehaviorEventBus>();

        services.AddSingleton<CharacterState>();

        services.AddSingleton<
            IBehaviorEngine,
            BehaviorEngine>();

        services.AddSingleton<
            IBehaviorRule,
            UserIdleRule>();

        services.AddSingleton<
            IBehaviorRule,
            UserActiveRule>();

        services.AddSingleton<
            IBehaviorRule,
            StartupRule>();

        services.AddSingleton<
            IActionDispatcher,
            ActionDispatcher>();

        services.AddHostedService<
            BehaviorHostedService>();

        services.AddSingleton<
            IBehaviorScheduler,
            BehaviorScheduler>();

        services.AddSingleton<
            IUserIdleDetector,
            WindowsUserIdleDetector>();

        services.AddSingleton<
            IIdleMonitor,
            IdleMonitor>();

        services.AddSingleton<
            IBehaviorEventSource,
            IdleEventSource>();

        services.AddSingleton<
            IBehaviorContext,
            BehaviorContext>();

        services.AddSingleton<
            ICharacterContext,
            CharacterContext>();

        services.AddSingleton<
            IActionExecutor,
            SleepActionExecutor>();

        services.AddSingleton<
            IActionExecutor,
            WakeUpActionExecutor>();

        services.AddSingleton<
            IActionExecutor,
            IdleActionExecutor>();

        services.AddSingleton<
            IActionExecutor,
            TalkActionExecutor>();

        services.AddSingleton<
            IAikoStateService,
            AikoStateService>();

        services.AddSingleton<
            BehaviorDebugService>();

        services.AddSingleton<IEmotionEngine, EmotionEngine>();

        services.AddSingleton<
            IMoodManager,
            MoodManager>();

        services.AddSingleton<
            IPersonalityService,
            PersonalityService>();

        return services;
    }
}