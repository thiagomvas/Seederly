namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents the reusable components in an OpenAPI document, such as schemas, responses, and parameters.
/// </summary>
public class OpenApiComponents
{
    /// <summary>
    /// A dictionary of reusable schema definitions by name.
    /// </summary>
    public Dictionary<string, OpenApiSchema> Schemas { get; set; } = new();
}
