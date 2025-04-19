namespace Seederly.Core;

public class Workspace
{
    public string Path { get; set; }
    public string Name { get; set; }
    
    public List<ApiEndpoint> Endpoints { get; set; } = new();
    
    public Workspace(string name)
    {
        Name = name;
    }
}