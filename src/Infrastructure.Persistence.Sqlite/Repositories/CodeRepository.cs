using System.Data;
using Infrastructure.Persistence.Constants;
using Microsoft.Data.Sqlite;
using Shared.Helpers;

namespace Infrastructure.Persistence.Repositories;

public class CodeRepository : ICodeRepository
{
    private readonly SqliteConnection _connection;

    public CodeRepository(SqliteConnection connection)
    {
        _connection = connection;
    }
    
    public bool AddRange(int count, byte length)
    {
        EnsureConnectionIsOpen();
        var transaction = _connection.BeginTransaction();

        try
        {
            for (int i = 0; i < count; i++)
            {
                string code = RandomGeneratorHelper.Generate(length);

                while (Exists(code))
                {
                    code = RandomGeneratorHelper.Generate(length);
                }

                var command = _connection.CreateCommand();
                command.CommandText = SqlConstants.InsertDiscountStatement;
                command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
                command.Parameters.AddWithValue("@code", code);
                command.ExecuteNonQuery();
            }

            transaction.Commit();

            return true;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return false;
        }
    }

    public bool Exists(string code)
    {
        EnsureConnectionIsOpen();
        
        var select = _connection.CreateCommand();
        select.CommandText = SqlConstants.CheckIfCodeExistsStatement;
        select.Parameters.AddWithValue("@code", code);
        
        var result = (long)(select.ExecuteScalar() ?? 0);
        
        return result == 1;
    }

    public bool UseCode(string code)
    {
        EnsureConnectionIsOpen();
        
        var update = _connection.CreateCommand();
        update.CommandText = SqlConstants.UpdateCodeStatement;
        update.Parameters.AddWithValue("@code", code);
        
        var result = update.ExecuteNonQuery();
        _connection.Close();

        return result > 0;
    }

    public bool IsUsed(string code)
    {
        EnsureConnectionIsOpen();
        
        var command = _connection.CreateCommand();
        command.CommandText = SqlConstants.SelectUsedCodeStatement;
        command.Parameters.AddWithValue("@code", code);

        var result = (long)(command.ExecuteScalar() ?? 0) > 0;
        _connection.Close();
        
        return result;
    }

    private void EnsureConnectionIsOpen()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }
}