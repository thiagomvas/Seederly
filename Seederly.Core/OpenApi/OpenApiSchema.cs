using System.Text.Json.Serialization;

namespace Seederly.Core.OpenApi;

/// <summary>
/// Represents a schema object in an OpenAPI definition.
/// </summary>
public class OpenApiSchema
{
    /// <summary>
    /// The type of the schema (e.g., object, string, array).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The format of the schema type (e.g., int32, int64, date-time).
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// The properties defined for the schema if it is an object.
    /// </summary>
    public Dictionary<string, OpenApiSchema> Properties { get; set; } = new();

    /// <summary>
    /// The list of required property names.
    /// </summary>
    public List<string> Required { get; set; } = new();

    /// <summary>
    /// The schema of items if the type is an array.
    /// </summary>
    public OpenApiSchema? Items { get; set; }

    /// <summary>
    /// A reference to another schema definition using $ref.
    /// </summary>
    [JsonPropertyName("$ref")]
    public string Ref { get; set; } = string.Empty;

    /// <summary>
    /// A description of the schema.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    private static object GenerateExample(OpenApiSchema schema)
    {
        if (!string.IsNullOrEmpty(schema.Ref))
        {
            // You'd typically resolve $ref here using a schema registry or dictionary
            return new {}; // Placeholder
        }

        return schema.Type switch
        {
            "string" => "string",
            "number" => 0.0,
            "integer" => 0,
            "boolean" => false,
            "array" => new[] { GenerateExample(schema.Items ?? new OpenApiSchema { Type = "string" }) },
            "object" => schema.Properties.ToDictionary(
                prop => prop.Key,
                prop => GenerateExample(prop.Value)),
            _ => null
        };
    }

    public string GenerateJsonBody()
    {
        var example = GenerateExample(this);
        return System.Text.Json.JsonSerializer.Serialize(example, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

}
