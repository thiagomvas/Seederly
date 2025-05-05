namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents the request body in an OpenAPI definition.
/// </summary>
public class OpenApiRequestBody
{
    /// <summary>
    /// The content of the request body, mapped by media type.
    /// </summary>
    public Dictionary<string, OpenApiMediaType> Content { get; set; }
}
