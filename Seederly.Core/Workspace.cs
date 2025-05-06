using System.Text.Json;
using Seederly.Core.Automation;
using Seederly.Core.Converters;
using Seederly.Core.OpenApi;

namespace Seederly.Core;

/// <summary>
/// Represents a workspace that contains API endpoints.
/// </summary>
public class Workspace
{
    /// <summary>
    /// Gets or sets the path where the workspace is stored.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the name of the workspace.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the API endpoints in the workspace.
    /// </summary>
    public List<ApiEndpoint> Endpoints { get; set; } = new();

    public List<Workflow> Workflows { get; set; } = new();

    public Workspace(string name)
    {
        Name = name;
    }

    public string SerializeToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            Converters =
            {
                new InjectionVariableTargetEnumConverter(),
                new ExtractionVariableTargetEnumConverter()
            }
        });
    }

    public static Workspace DeserializeFromJson(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<Workspace>(json, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            Converters =
            {
                new InjectionVariableTargetEnumConverter(),
                new ExtractionVariableTargetEnumConverter()
            }
        }) ?? new Workspace("Default");
    }

    public static Workspace CreateFromOpenApiDocument(OpenApiDocument document)
    {
        var result = new Workspace(document.Info.Title);
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                var name = !string.IsNullOrWhiteSpace(operation.Value.Summary)
                    ? operation.Value.Summary
                    : $"{operation.Key.ToUpperInvariant()} {path.Key}";

                var schemaName = operation.Value.RequestBody?.Content.FirstOrDefault().Value.Schema?.Ref.Split('/')
                    .Last();
                var schema = document.Components?.Schemas.FirstOrDefault(x => x.Key == schemaName).Value;

                var apiEndpoint = new ApiEndpoint
                {
                    Name = name,
                    Request = new ApiRequest()
                    {
                        Method = HttpMethod.Parse(operation.Key),
                        Url = System.IO.Path.Combine(document.Servers.FirstOrDefault()?.Url ?? "{baseUrl}", path.Key),
                        Body = schema?.GenerateJsonBody(),
                    }
                };
                if (apiEndpoint.Request.Method != HttpMethod.Get)
                {
                    apiEndpoint.Schema = GenerateEndpointSchemaFromOpenApiSchema(schema);
                }

                result.Endpoints.Add(apiEndpoint);
            }
        }

        return result;
    }

    private static Dictionary<string, string> GenerateEndpointSchemaFromOpenApiSchema(OpenApiSchema? schema)
    {
        var result = new Dictionary<string, string>();

        if (schema != null && schema.Properties != null)
        {
            foreach (var property in schema.Properties)
            {
                var propertyName = property.Key;
                var propertySchema = property.Value;
                
                string seederlyGenKey = string.Empty;

                if (propertySchema.Type == "object")
                {
                    var nestedProperties = GenerateEndpointSchemaFromOpenApiSchema(propertySchema);
                    foreach (var nestedProperty in nestedProperties)
                    {
                        result.Add($"{propertyName}.{nestedProperty.Key}", nestedProperty.Value);
                    }
                }
                else
                {
                        result.Add(propertyName, GetSchemaKey(FakeRequestFactory.Instance, propertyName, propertySchema.Type));
                }
            }
        }

        return result;
    }

    private static string GetSchemaKey(FakeRequestFactory factory, string name, string type)
    {
        var key = factory.Generators.Keys
            .FirstOrDefault(k => k.Split('.').Last().Equals(name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(key))
            return key;

        return type switch 
        {
            "string" => "lorem.sentence",
            "integer" => "random.number",
            "number" => "random.number",
            "boolean" => "random.bool",
            _ => "lorem.sentence"
        };
    }


}