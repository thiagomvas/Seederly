namespace Seederly.Core;

/// <summary>
/// Represents an API endpoint.
/// </summary>
public class ApiEndpoint
{
    /// <summary>
    /// Gets or sets the name of the API endpoint.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the request associated with the API endpoint.
    /// </summary>
    public ApiRequest Request { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the schema of the API endpoint, represented as a dictionary of key-value pairs used to generate a body for the <see cref="Request"/>.
    /// </summary>
    public Dictionary<string, string> Schema { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the children of the API endpoint, which are also API endpoints.
    /// </summary>
    public List<ApiEndpoint> Children { get; set; } = new();
    
    public ApiEndpoint()
    {
    }
    
    
    public ApiEndpoint(string name, ApiRequest request)
    {
        Name = name;
        Request = request;
    }
}