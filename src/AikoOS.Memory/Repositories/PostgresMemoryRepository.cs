using AikoOS.Memory.Database;
using AikoOS.Memory.Models;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace AikoOS.Memory.Repositories;

public sealed class PostgresMemoryRepository : IMemoryRepository
{
    private readonly PostgresDataSourceFactory _dataSourceFactory;
    private readonly ILogger<PostgresMemoryRepository> _logger;

    public PostgresMemoryRepository(
        PostgresDataSourceFactory dataSourceFactory,
        ILogger<PostgresMemoryRepository> logger)
    {
        _dataSourceFactory = dataSourceFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MemoryEntry>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        List<MemoryEntry> memories = [];

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                SELECT
                    id,
                    content,
                    created_at,
                    updated_at
                FROM memories
                ORDER BY created_at DESC;
                """);

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            memories.Add(new MemoryEntry
            {
                Id = reader.GetInt64(0),
                Content = reader.GetString(1),
                CreatedAt = reader.GetFieldValue<DateTimeOffset>(2),
                UpdatedAt = reader.GetFieldValue<DateTimeOffset>(3)
            });
        }

        _logger.LogInformation(
            "Loaded {MemoryCount} memory entries.",
            memories.Count);

        return memories;
    }

    public async Task<MemoryEntry> AddAsync(
        string content,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        string normalizedContent = content.Trim();

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                INSERT INTO memories
                (
                    content
                )
                VALUES
                (
                    $1
                )
                RETURNING
                    id,
                    content,
                    created_at,
                    updated_at;
                """);

        command.Parameters.Add(
            new NpgsqlParameter<string>
            {
                TypedValue = normalizedContent,
                NpgsqlDbType = NpgsqlDbType.Text
            });

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException(
                "PostgreSQL did not return the created memory.");
        }

        MemoryEntry memory = new()
        {
            Id = reader.GetInt64(0),
            Content = reader.GetString(1),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(2),
            UpdatedAt = reader.GetFieldValue<DateTimeOffset>(3)
        };

        _logger.LogInformation(
            "Created memory entry {MemoryId}.",
            memory.Id);

        return memory;
    }

    public async Task DeleteAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                DELETE FROM memories
                WHERE id = $1;
                """);

        command.Parameters.Add(
            new NpgsqlParameter<long>
            {
                TypedValue = id,
                NpgsqlDbType = NpgsqlDbType.Bigint
            });

        int affectedRows =
            await command.ExecuteNonQueryAsync(
                cancellationToken);

        if (affectedRows == 0)
        {
            _logger.LogWarning(
                "Memory entry {MemoryId} was not found.",
                id);

            return;
        }

        _logger.LogInformation(
            "Deleted memory entry {MemoryId}.",
            id);
    }
}