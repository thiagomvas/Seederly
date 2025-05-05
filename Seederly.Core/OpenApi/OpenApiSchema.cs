namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a schema object in an OpenAPI definition.
/// </summary>
public class OpenApiSchema
{
    /// <summary>
    /// The type of the schema (e.g., object, string, array).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The format of the schema type (e.g., int32, int64, date-time).
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// The properties defined for the schema if it is an object.
    /// </summary>
    public Dictionary<string, OpenApiSchema> Properties { get; set; }

    /// <summary>
    /// The list of required property names.
    /// </summary>
    public List<string> Required { get; set; }

    /// <summary>
    /// The schema of items if the type is an array.
    /// </summary>
    public OpenApiSchema Items { get; set; }

    /// <summary>
    /// A reference to another schema definition using $ref.
    /// </summary>
    public string Ref { get; set; }

    /// <summary>
    /// A description of the schema.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// An example value for the schema.
    /// </summary>
    public string Example { get; set; }
}
