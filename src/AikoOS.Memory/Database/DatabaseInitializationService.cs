using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AikoOS.Memory.Database;

public sealed class DatabaseInitializationService : IHostedService
{
    private readonly PostgresDataSourceFactory _dataSourceFactory;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        PostgresDataSourceFactory dataSourceFactory,
        ILogger<DatabaseInitializationService> logger)
    {
        _dataSourceFactory = dataSourceFactory;
        _logger = logger;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Initializing the AikoOS PostgreSQL database...");

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                CREATE TABLE IF NOT EXISTS memories
                (
                    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                    content TEXT NOT NULL,
                    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
                );

                CREATE INDEX IF NOT EXISTS ix_memories_created_at
                ON memories (created_at DESC);
                """);

        await command.ExecuteNonQueryAsync(
            cancellationToken);

        _logger.LogInformation(
            "AikoOS PostgreSQL database initialization completed.");
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}