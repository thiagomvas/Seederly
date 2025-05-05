namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a server in an OpenAPI definition.
/// </summary>
public class OpenApiServer
{
    /// <summary>
    /// The URL of the server.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// A description of the server.
    /// </summary>
    public string Description { get; set; }
}
