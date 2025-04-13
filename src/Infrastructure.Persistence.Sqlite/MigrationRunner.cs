using System.Reflection;
using Infrastructure.Persistence.Constants;
using Microsoft.Data.Sqlite;
using Shared;
using Shared.Attributes;

namespace Infrastructure.Persistence;

internal class MigrationRunner
{
    private readonly string _connectionString;

    public MigrationRunner(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Run()
    {
        var migrations = typeof(IInfrastructureLayerMarker).Assembly.GetTypes()
            .Where(t => typeof(IMigration).IsAssignableFrom(t)
                        && !t.IsInterface && !t.IsAbstract &&
                        t.GetCustomAttribute<MigrationAttribute>() != null).Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttribute<MigrationAttribute>()
            })
            .OrderBy(m => m.Attribute.Order);

        using var connection = new SqliteConnection(_connectionString);
        
        connection.Open();

        EnsureMigrationsTable(connection);

        foreach (var migrationItem in migrations)
        {
            ApplyMigration(new(migrationItem.Type, migrationItem.Attribute), connection);
        }
    }

    private void ApplyMigration((Type Type, MigrationAttribute Attribute) item, SqliteConnection connection)
    {
        var migration = (IMigration)Activator.CreateInstance(item.Type, connection)!;

        if (!IsMigrationApplied(connection, item.Attribute.Name))
        {
            migration.Apply();
            RecordMigration(connection, item.Attribute.Name);
        }
    }

    private bool IsMigrationApplied(SqliteConnection connection, string name)
    {
        using var command = connection.CreateCommand();
        command.CommandText = SqlConstants.SelectMigrationsCountByName;
        
        command.Parameters.Add(new SqliteParameter("name", name));

        return (long)(command.ExecuteScalar() ?? 0) > 0;
    }
        
    private void RecordMigration(SqliteConnection connection, string migrationName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = SqlConstants.InsertMigrationStatement;
        
        command.Parameters.Add(new SqliteParameter("id", Guid.NewGuid().ToString()));
        command.Parameters.Add(new SqliteParameter("name", migrationName));

        command.ExecuteNonQuery();
    }
    
    private void EnsureMigrationsTable(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = SqlConstants.CreateMigrationsTableIfNotExists;

        command.ExecuteNonQuery();
    }
}