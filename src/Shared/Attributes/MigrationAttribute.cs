namespace Shared.Attributes;

public class MigrationAttribute : Attribute
{
    public string Name { get; }
    public int Order { get; }
    
    public MigrationAttribute(string name, int order)
    {
        Name = name;
        Order = order;
    }
}