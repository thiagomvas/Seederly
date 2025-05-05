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
        
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                var name = !string.IsNullOrWhiteSpace(operation.Value.Summary)
                    ? operation.Value.Summary
                    : $"{operation.Key.ToString().ToUpperInvariant()} {path.Key}";
                var apiEndpoint = new ApiEndpoint
                {
                    Name = name,
                    Request = new ApiRequest()
                    {
                        Method = HttpMethod.Parse(operation.Key.ToString()),
                        Url = path.Key,
                    }
                };
                
                result.Endpoints.Add(apiEndpoint);
            }
        }

        return result;
    }
}