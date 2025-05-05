namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a path item in an OpenAPI definition, containing HTTP operations (e.g., GET, POST, etc.).
/// </summary>
public class OpenApiPathItem
{
    /// <summary>
    /// A dictionary of HTTP operations for the path (e.g., "get", "post", etc.).
    /// </summary>
    public Dictionary<string, OpenApiOperation> Operations { get; set; }
}
