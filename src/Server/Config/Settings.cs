namespace Server.Config;

public class Settings
{
    public int Port { get; init; }
    public string[] WhiteListedIpAddresses { get; init; } = [];
    public string ConnectionString { get; init; } = string.Empty;
    public CodeRules CodeRules { get; init; } = default!;
}