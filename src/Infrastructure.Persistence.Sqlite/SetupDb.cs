namespace Infrastructure.Persistence;

public class SetupDb
{
    public SetupDb(string connectionString)
    {
        new MigrationRunner(connectionString).Run();
    }
}