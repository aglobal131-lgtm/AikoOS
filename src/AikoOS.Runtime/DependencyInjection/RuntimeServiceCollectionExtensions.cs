using AikoOS.Runtime.Services;
using AikoOS.Runtime.Planning;
using AikoOS.Runtime.Hosting;
using AikoOS.Runtime.Brain;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.Runtime.DependencyInjection;

public static class RuntimeServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSRuntime(
        this IServiceCollection services)
    {
        services.AddSingleton<IRuntimeState, RuntimeState>();

        services.AddHostedService<AikoRuntimeHostedService>();

        services.AddSingleton<IPlanActionMapper, PlanActionMapper>();

        services.AddSingleton<PlanExecutionService>();

        services.AddSingleton<IBrainRuntimeService, BrainRuntimeService>();

        // services.AddHostedService<BrainStartupService>();

        services.AddSingleton<IBrainRequestService, BrainRequestService>();




        return services;
    }
}