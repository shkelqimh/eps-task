namespace Infrastructure.Persistence.Repositories;

public interface ICodeRepository
{
    bool AddRange(int count, byte codeLength);
    bool IsUsed(string code);
    bool Exists(string code);
    bool UseCode(string code);
}