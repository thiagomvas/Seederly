using System.Text.Json;
using Seederly.Core.Automation;
using Seederly.Core.Configuration;
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

    /// <summary>
    /// Gets or sets the workflows in the workspace.
    /// </summary>
    public List<Workflow> Workflows { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the staging environments for the workspace.
    /// </summary>
    public Dictionary<string, StagingEnvironment> StagingEnvironments { get; set; } = new()
    {
        { "Production", new StagingEnvironment() },
    };

    public Workspace()
    {
        
    }
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
        
        var baseUrl = new Uri(document.Servers.FirstOrDefault()?.Url ?? "{baseUrl}");

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
                        Url = new Uri(baseUrl, path.Key).ToString(),
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
                    result.Add(propertyName, GetSchemaKey(FakeRequestFactory.Instance, propertyName, propertySchema));
                }
            }
        }

        return result;
    }

    private static string GetSchemaKey(FakeRequestFactory factory, string name, OpenApiSchema schema)
    {
        switch (schema.Format)
        {
            case "email":
                return "internet.email";
            case "uuid":
                return "random.uuid";
            case "date-time":
            case "date":
                return "date.past";
        }
        
        var key = factory.Generators.Keys
            .FirstOrDefault(k => k.Split('.').Last().Equals(name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(key))
            return key;

        switch (schema.Type)
        {
            case "integer":
            case "number":
                return "random.number";
            case "boolean":
                return "random.bool";
        }
        
        // Try to get a gen key that contains the entire name
        key = factory.Generators.Keys
            .FirstOrDefault(k => k.Contains(name, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(key))
            return key;

        return "lorem.sentence";
    }
}