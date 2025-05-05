namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a media type in an OpenAPI definition, describing the schema of the content.
/// </summary>
public class OpenApiMediaType
{
    /// <summary>
    /// The schema that defines the structure and constraints of the media type's content.
    /// </summary>
    public OpenApiSchema? Schema { get; set; }
}

