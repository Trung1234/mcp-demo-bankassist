namespace BankAssist.Mcp.Configuration;

public sealed class DataOptions
{
    public const string SectionName = "Data";

    public string Directory { get; set; } = "data";
}
