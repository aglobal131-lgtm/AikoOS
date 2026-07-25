using AikoOS.Memory.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AikoOS.Memory.Database;

public sealed class PostgresDataSourceFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresDataSourceFactory(
        IOptions<DatabaseOptions> databaseOptions)
    {
        DatabaseOptions options = databaseOptions.Value;

        NpgsqlConnectionStringBuilder connectionStringBuilder = new()
        {
            Host = options.Host,
            Port = options.Port,
            Database = options.Database,
            Username = options.Username,
            Password = options.Password,

            Pooling = true,
            Timeout = 10,
            CommandTimeout = 30
        };

        NpgsqlDataSourceBuilder dataSourceBuilder = new(
            connectionStringBuilder.ConnectionString);

        _dataSource = dataSourceBuilder.Build();
    }

    public NpgsqlDataSource DataSource => _dataSource;
}