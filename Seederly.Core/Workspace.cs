using Seederly.Core.Automation;

namespace Seederly.Core;

/// <summary>
/// Represents a workspace that contains API endpoints.
/// </summary>
public class Workspace
{
    /// <summary>
    /// Gets or sets the path where the workspace is stored.
    /// </summary>
    public string Path { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the workspace.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the API endpoints in the workspace.
    /// </summary>
    public List<ApiEndpoint> Endpoints { get; set; } = new();
    
    public List<Workflow> Workflows { get; set; } = new();
    
    public Workspace(string name)
    {
        Name = name;
    }
}