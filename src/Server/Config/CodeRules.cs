namespace Server.Config;

public class CodeRules
{
    public int MaxCodeGenerationWithinRequestAllowed { get; init; }
    public int MinCodeLength { get; init; }
    public int MaxCodeLength { get; init; }
}