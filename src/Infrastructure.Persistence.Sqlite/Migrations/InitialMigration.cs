using Infrastructure.Persistence.Constants;
using Microsoft.Data.Sqlite;
using Shared;
using Shared.Attributes;

namespace Infrastructure.Persistence.Migrations;

[Migration("InitialMigration", order: 1)]
public class InitialMigration : IMigration
{
    private readonly SqliteConnection _connection;

    public InitialMigration(SqliteConnection connection)
    {
        _connection = connection;
    }
    
    public void Apply()
    {
        _connection.Open();

        using var command = _connection.CreateCommand();

        command.CommandText = SqlConstants.CreateDiscountsTableIfNotExists;

        command.ExecuteNonQuery();
    }
}