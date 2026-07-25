using AikoOS.Core.Interfaces;
using AikoOS.Core.Models;
using AikoOS.Memory.Database;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace AikoOS.Infrastructure.Repositories;

public sealed class PostgreSqlChatRepository : IChatRepository
{
    private readonly PostgresDataSourceFactory _dataSourceFactory;
    private readonly ILogger<PostgreSqlChatRepository> _logger;

    public PostgreSqlChatRepository(
        PostgresDataSourceFactory dataSourceFactory,
        ILogger<PostgreSqlChatRepository> logger)
    {
        _dataSourceFactory = dataSourceFactory;
        _logger = logger;
    }

    public async Task<ChatConversation> CreateConversationAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Guid conversationId = Guid.NewGuid();

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                INSERT INTO chat_conversations
                (
                    id,
                    title
                )
                VALUES
                (
                    $1,
                    $2
                )
                RETURNING
                    id,
                    title,
                    created_at,
                    updated_at;
                """);

        command.Parameters.Add(
            new NpgsqlParameter<Guid>
            {
                TypedValue = conversationId,
                NpgsqlDbType = NpgsqlDbType.Uuid
            });

        command.Parameters.Add(
            new NpgsqlParameter<string>
            {
                TypedValue = title.Trim(),
                NpgsqlDbType = NpgsqlDbType.Varchar
            });

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException(
                "PostgreSQL did not return the created conversation.");
        }

        ChatConversation conversation =
            ReadConversation(reader);

        _logger.LogInformation(
            "Created chat conversation {ConversationId}.",
            conversation.Id);

        return conversation;
    }

    public async Task<IReadOnlyList<ChatConversation>>
        GetConversationsAsync(
            CancellationToken cancellationToken = default)
    {
        List<ChatConversation> conversations = [];

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                SELECT
                    id,
                    title,
                    created_at,
                    updated_at
                FROM chat_conversations
                ORDER BY updated_at DESC;
                """);

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            conversations.Add(ReadConversation(reader));
        }

        _logger.LogInformation(
            "Loaded {ConversationCount} chat conversations.",
            conversations.Count);

        return conversations;
    }

    public async Task<IReadOnlyList<StoredChatMessage>>
        GetMessagesAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default)
    {
        List<StoredChatMessage> messages = [];

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                SELECT
                    id,
                    conversation_id,
                    role,
                    content,
                    created_at
                FROM chat_messages
                WHERE conversation_id = $1
                ORDER BY created_at ASC;
                """);

        command.Parameters.Add(
            new NpgsqlParameter<Guid>
            {
                TypedValue = conversationId,
                NpgsqlDbType = NpgsqlDbType.Uuid
            });

        await using NpgsqlDataReader reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            messages.Add(ReadMessage(reader));
        }

        _logger.LogInformation(
            "Loaded {MessageCount} messages for conversation {ConversationId}.",
            messages.Count,
            conversationId);

        return messages;
    }

    public async Task<StoredChatMessage> AddMessageAsync(
        Guid conversationId,
        string role,
        string content,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        Guid messageId = Guid.NewGuid();

        await using NpgsqlConnection connection =
            await _dataSourceFactory.DataSource
                .OpenConnectionAsync(cancellationToken);

        await using NpgsqlTransaction transaction =
            await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await using NpgsqlCommand insertCommand =
                new(
                    """
                    INSERT INTO chat_messages
                    (
                        id,
                        conversation_id,
                        role,
                        content
                    )
                    VALUES
                    (
                        $1,
                        $2,
                        $3,
                        $4
                    )
                    RETURNING
                        id,
                        conversation_id,
                        role,
                        content,
                        created_at;
                    """,
                    connection,
                    transaction);

            insertCommand.Parameters.Add(
                new NpgsqlParameter<Guid>
                {
                    TypedValue = messageId,
                    NpgsqlDbType = NpgsqlDbType.Uuid
                });

            insertCommand.Parameters.Add(
                new NpgsqlParameter<Guid>
                {
                    TypedValue = conversationId,
                    NpgsqlDbType = NpgsqlDbType.Uuid
                });

            insertCommand.Parameters.Add(
                new NpgsqlParameter<string>
                {
                    TypedValue = role.Trim(),
                    NpgsqlDbType = NpgsqlDbType.Varchar
                });

            insertCommand.Parameters.Add(
                new NpgsqlParameter<string>
                {
                    TypedValue = content.Trim(),
                    NpgsqlDbType = NpgsqlDbType.Text
                });

            StoredChatMessage storedMessage;

            await using (
                NpgsqlDataReader reader =
                    await insertCommand.ExecuteReaderAsync(
                        cancellationToken))
            {
                if (!await reader.ReadAsync(cancellationToken))
                {
                    throw new InvalidOperationException(
                        "PostgreSQL did not return the created chat message.");
                }

                storedMessage = ReadMessage(reader);
            }

            await using NpgsqlCommand updateCommand =
                new(
                    """
                    UPDATE chat_conversations
                    SET updated_at = NOW()
                    WHERE id = $1;
                    """,
                    connection,
                    transaction);

            updateCommand.Parameters.Add(
                new NpgsqlParameter<Guid>
                {
                    TypedValue = conversationId,
                    NpgsqlDbType = NpgsqlDbType.Uuid
                });

            await updateCommand.ExecuteNonQueryAsync(
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Created chat message {MessageId} for conversation {ConversationId}.",
                storedMessage.Id,
                conversationId);

            return storedMessage;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateConversationTitleAsync(
        Guid conversationId,
        string title,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                UPDATE chat_conversations
                SET
                    title = $1,
                    updated_at = NOW()
                WHERE id = $2;
                """);

        command.Parameters.Add(
            new NpgsqlParameter<string>
            {
                TypedValue = title.Trim(),
                NpgsqlDbType = NpgsqlDbType.Varchar
            });

        command.Parameters.Add(
            new NpgsqlParameter<Guid>
            {
                TypedValue = conversationId,
                NpgsqlDbType = NpgsqlDbType.Uuid
            });

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        await using NpgsqlCommand command =
            _dataSourceFactory.DataSource.CreateCommand(
                """
                DELETE FROM chat_conversations
                WHERE id = $1;
                """);

        command.Parameters.Add(
            new NpgsqlParameter<Guid>
            {
                TypedValue = conversationId,
                NpgsqlDbType = NpgsqlDbType.Uuid
            });

        await command.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation(
            "Deleted chat conversation {ConversationId}.",
            conversationId);
    }

    private static ChatConversation ReadConversation(
        NpgsqlDataReader reader)
    {
        return new ChatConversation
        {
            Id = reader.GetGuid(0),
            Title = reader.GetString(1),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(2),
            UpdatedAt = reader.GetFieldValue<DateTimeOffset>(3)
        };
    }

    private static StoredChatMessage ReadMessage(
        NpgsqlDataReader reader)
    {
        return new StoredChatMessage
        {
            Id = reader.GetGuid(0),
            ConversationId = reader.GetGuid(1),
            Role = reader.GetString(2),
            Content = reader.GetString(3),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(4)
        };
    }
}