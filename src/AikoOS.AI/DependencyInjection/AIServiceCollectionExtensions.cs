using AikoOS.AI.Interfaces;
using AikoOS.AI.Options;
using AikoOS.AI.Providers.Gemini;
using AikoOS.AI.Brain.Decisions;
using AikoOS.AI.Brain.Planning;
using AikoOS.AI.Brain.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AikoOS.AI.DependencyInjection;

public static class AIServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AIOptions>()
            .Bind(
                configuration.GetSection(
                    AIOptions.SectionName))
            .Validate(
                options =>
                    string.Equals(
                        options.Provider,
                        "Gemini",
                        StringComparison.OrdinalIgnoreCase),
                "AI provider must be Gemini.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(
                        options.Model),
                "AI model is required.")
            .Validate(
                options =>
                    Uri.TryCreate(
                        options.Endpoint,
                        UriKind.Absolute,
                        out _),
                "AI endpoint must be a valid absolute URL.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(
                        options.ApiKey),
                "Gemini API key is required.")
            .ValidateOnStart();

        services.AddHttpClient<
            IChatProvider,
            GeminiChatProvider>(
            (serviceProvider, client) =>
            {
                AIOptions options =
                    serviceProvider
                        .GetRequiredService<
                            IOptions<AIOptions>>()
                        .Value;

                client.BaseAddress =
                    new Uri(
                        options.Endpoint.TrimEnd('/')
                        + "/");

                client.Timeout =
                    TimeSpan.FromMinutes(2);
            });

        services.AddSingleton<IDecisionMaker, GeminiDecisionMaker>();

        services.AddSingleton<IPlanningService, PlanningService>();

        services.AddHttpClient<IChatProvider, GeminiChatProvider>(client => { client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/"); });

        services.AddSingleton<GeminiDecisionParser>();



        return services;
    }
}