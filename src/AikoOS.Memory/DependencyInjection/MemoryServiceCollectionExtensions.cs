using AikoOS.Memory.Database;
using AikoOS.Memory.Options;
using AikoOS.Memory.Services;
using AikoOS.Memory.Repositories;
using AikoOS.Memory.Interfaces;
using AikoOS.Memory.Influence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.Memory.DependencyInjection;

public static class MemoryServiceCollectionExtensions
{
    public static IServiceCollection AddAikoOSMemory(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(
                configuration.GetSection(
                    DatabaseOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Host),
                "Database host is required.")
            .Validate(
                options =>
                    options.Port is > 0 and <= 65535,
                "Database port is invalid.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Database),
                "Database name is required.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Username),
                "Database username is required.")
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Password),
                "Database password is required.")
            .ValidateOnStart();

        services.AddSingleton<PostgresDataSourceFactory>();

        services.AddSingleton<
            IMemoryRepository,
            PostgresMemoryRepository>();

        services.AddSingleton<
            IMemoryService,
            MemoryService>();

        services.AddSingleton<IMemoryPipeline, MemoryPipeline>();

        services.AddHostedService<PostgresConnectionTestService>();

        services.AddHostedService<DatabaseInitializationService>();

        services.AddSingleton<IMemoryInfluenceService, MemoryInfluenceService>();

        return services;
    }
}