namespace Infrastructure.Persistence.Constants;

internal class SqlConstants
{
    public const string CreateMigrationsTableIfNotExists =
        "CREATE TABLE IF NOT EXISTS Migrations(Id TEXT PRIMARY KEY, Name TEXT NOT NULL UNIQUE, AppliedOn DEFAULT CURRENT_TIMESTAMP);";

    public const string CreateDiscountsTableIfNotExists =
        "CREATE TABLE IF NOT EXISTS Discounts(Id TEXT PRIMARY KEY, Code NOT NULL UNIQUE, Used INTEGER DEFAULT 0, CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP)";

    public const string InsertDiscountStatement =
        "INSERT INTO Discounts (Id, Code) VALUES (@id, @code)";

    public const string InsertMigrationStatement = "INSERT INTO Migrations (Id, Name) VALUES (@id, @name);";
    
    public const string SelectUsedCodeStatement = "SELECT Used FROM Discounts WHERE Code = @code";

    public const string SelectMigrationsCountByName = "SELECT COUNT(*) FROM Migrations WHERE Name = @name;";

    public const string UpdateCodeStatement = "UPDATE Discounts SET Used = 1 WHERE Code = @code";

    public const string CheckIfCodeExistsStatement =
        "SELECT CASE WHEN EXISTS (SELECT 1 FROM Discounts WHERE Code = @code) THEN 1 ELSE 0 END";
}