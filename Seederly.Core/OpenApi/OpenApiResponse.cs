namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a response in an OpenAPI definition.
/// </summary>
public class OpenApiResponse
{
    /// <summary>
    /// A description of the response.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The content of the response, mapped by media type.
    /// </summary>
    public Dictionary<string, OpenApiMediaType> Content { get; set; }
}
