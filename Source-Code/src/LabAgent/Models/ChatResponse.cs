namespace LabAgent.Models;

public class ChatResponse
{
    public string Reply { get; set; } = string.Empty;
    public List<string> ToolsInvoked { get; set; } = new();
    public bool HasError { get; set; }
    public string? Error { get; set; }
}
