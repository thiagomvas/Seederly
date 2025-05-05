namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a parameter in an OpenAPI operation.
/// </summary>
public class OpenApiParameter
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The location of the parameter (e.g., query, path, header, cookie).
    /// </summary>
    public string In { get; set; }

    /// <summary>
    /// Indicates whether the parameter is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// The schema that defines the parameter's data type and constraints.
    /// </summary>
    public OpenApiSchema Schema { get; set; }
}
