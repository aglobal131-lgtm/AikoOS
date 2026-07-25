using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AikoOS.Memory.Database;

public sealed class PostgresConnectionTestService : IHostedService
{
    private readonly PostgresDataSourceFactory _dataSourceFactory;
    private readonly ILogger<PostgresConnectionTestService> _logger;

    public PostgresConnectionTestService(
        PostgresDataSourceFactory dataSourceFactory,
        ILogger<PostgresConnectionTestService> logger)
    {
        _dataSourceFactory = dataSourceFactory;
        _logger = logger;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Testing PostgreSQL connection...");

        await using NpgsqlConnection connection =
            await _dataSourceFactory.DataSource.OpenConnectionAsync(
                cancellationToken);

        await using NpgsqlCommand command =
            connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                current_database(),
                current_user,
                version();
            """;

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException(
                "PostgreSQL did not return connection information.");
        }

        string databaseName = reader.GetString(0);
        string username = reader.GetString(1);
        string serverVersion = reader.GetString(2);

        _logger.LogInformation(
            "PostgreSQL connection successful. Database: {DatabaseName}, User: {Username}, Version: {ServerVersion}",
            databaseName,
            username,
            serverVersion);
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}