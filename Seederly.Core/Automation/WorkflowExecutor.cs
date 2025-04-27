using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using NotSupportedException = System.NotSupportedException;

namespace Seederly.Core.Automation;

/// <summary>
/// Service responsible for executing workflows.
/// </summary>
public class WorkflowExecutor
{
    private readonly ApiRequestExecutor _apiRequestExecutor;
    private readonly VariableContext _variableContext;
    private readonly Workspace _workspace;
    private Dictionary<string, ApiRequest> _endpointMap = new(StringComparer.OrdinalIgnoreCase);
    
    public WorkflowExecutor(ApiRequestExecutor apiRequestExecutor, VariableContext variableContext, Workspace workspace)
    {
        _apiRequestExecutor = apiRequestExecutor;
        _variableContext = variableContext;
        _workspace = workspace;
        
        foreach (var endpoint in workspace.Endpoints)
        {
            _endpointMap[endpoint.Name] = endpoint.Request;
        }
    }
    
    /// <summary>
    /// Executes the given workflow.
    /// </summary>
    /// <param name="workflow">The workflow to execute.</param>
    /// <returns>A <see cref="WorkflowResult"/> containing info about the execution.</returns>
    public async Task<WorkflowResult> ExecuteAsync(Workflow workflow)
    {
        var result = new WorkflowResult
        {
            WorkflowName = workflow.Name,
            WorkflowDescription = workflow.Description,
            Steps = new List<WorkflowStepResult>()
        };
        
        foreach (var step in workflow.Steps)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = _endpointMap[step.EndpointName];
            if (request is null)
            {
                var failedStep = new WorkflowStepResult
                {
                    Name = step.Name,
                    Status = WorkflowStepStatus.EndpointNotFound,
                    ErrorMessage = $"Endpoint '{step.EndpointName}' not found."
                };
                result.Steps.Add(failedStep);
                return result;
            }
            
            // Clone the request to avoid modifying the original
            var clonedRequest = request.Clone();
            
            // Inject variables
            foreach (var injection in step.Inject)
            {
                var variableValue = _variableContext[injection.Key];
                if(string.IsNullOrWhiteSpace(variableValue))
                {
                    var failedStep = new WorkflowStepResult
                    {
                        Name = step.Name,
                        Status = WorkflowStepStatus.VariableNotFound,
                        ErrorMessage = $"Variable '{injection.Key}' not found."
                    };
                    result.Steps.Add(failedStep);
                    return result;
                }
                switch (injection.Target)
                {
                    case InjectionVariableTarget.Header:
                        clonedRequest.Headers[injection.Key] = variableValue;
                        break;
                    case InjectionVariableTarget.Body:
                        if(!InjectIntoBody(clonedRequest, injection, variableValue, step, result)) return result;
                        break;
                    case InjectionVariableTarget.Query:
                        clonedRequest.QueryParameters[injection.Key] = variableValue;
                        break;
                    case InjectionVariableTarget.Endpoint:
                        clonedRequest.Url = clonedRequest.Url.Replace(injection.Key, variableValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown target '{injection.Target}' for variable injection.");
                }
            }
            
            var response = await _apiRequestExecutor.ExecuteAsync(clonedRequest);
            // Extract variables
            foreach (var extraction in step.Extract)
            {
                switch (extraction.Source)
                {
                    case ExtractionVariableTarget.Response:
                        if(!ExtractFromResponse(response, extraction, step, result)) return result;
                        break;
                    case ExtractionVariableTarget.Header:
                        if (!ExtractFromHeader(response, extraction, step, result)) return result;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            
            // Add step result
            var stepResult = new WorkflowStepResult
            {
                Name = step.Name,
                Status = response.IsSuccess ? WorkflowStepStatus.Success : WorkflowStepStatus.ApiFailure,
                Response = response,
            };
            
            if (!response.IsSuccess)
            {
                stepResult.ErrorMessage = $"API request failed with status code {response.StatusCode}.";
            }
            else
            {
                stepResult.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }
            
            result.Steps.Add(stepResult);
        }

        if(!result.IsSuccessful)
        {
            result.ErrorMessage = $"Workflow execution failed: '{result.Steps.Last().ErrorMessage}'.";
        }
        
        return result;
    }

    private bool ExtractFromHeader(ApiResponse response, VariableExtractionRule extraction, WorkflowStep step,
        WorkflowResult result)
    {
        if (response.Headers.TryGetValue(extraction.JsonPath, out var headerValue))
        {
            _variableContext[extraction.VariableName] = headerValue;
        }
        else
        {
            var failedStep = new WorkflowStepResult
            {
                Name = step.Name,
                Status = WorkflowStepStatus.ExtractionFailure,
                ErrorMessage = $"Failed to extract variable '{extraction.VariableName}' from response header."
            };
            result.Steps.Add(failedStep);
            return false;
        }

        return true;
    }

    private static bool InjectIntoBody(ApiRequest clonedRequest, VariableInjectionRule injection, string variableValue,
        WorkflowStep step, WorkflowResult result)
    {
        if (!string.IsNullOrWhiteSpace(clonedRequest.Body))
        {
            var bodyJson = JsonSerializer.Deserialize<JsonObject>(clonedRequest.Body);
            if (bodyJson != null)
            {
                SetJsonPathValue(bodyJson, injection.Path, variableValue);
                clonedRequest.Body = bodyJson.ToString();
            }
            else
            {
                var failedStep = new WorkflowStepResult
                {
                    Name = step.Name,
                    Status = WorkflowStepStatus.InjectionFailure,
                    ErrorMessage = $"Failed to inject variable '{injection.Key}' into body due to being in an invalid format."
                };
                result.Steps.Add(failedStep);
                return false;
            }
        }
        else
        {
            var failedStep = new WorkflowStepResult
            {
                Name = step.Name,
                Status = WorkflowStepStatus.InjectionFailure,
                ErrorMessage = $"Failed to inject variable '{injection.Key}' into body because it is empty or null."
            };
            result.Steps.Add(failedStep);
            return false;
        }

        return true;
    }

    private bool ExtractFromResponse(ApiResponse? response, VariableExtractionRule extraction, WorkflowStep step,
        WorkflowResult result)
    {
        JsonObject? responseJson = null;
        if (response != null)
        {
            responseJson = JsonSerializer.Deserialize<JsonObject>(response.Content);
        }
        if (responseJson != null)
        {
            var extractedValue = GetJsonPathValue(responseJson, extraction.JsonPath);

            if (extractedValue != null)
            {
                _variableContext[extraction.VariableName] = extractedValue.ToString();
            }
            else
            {
                var failedStep = new WorkflowStepResult
                {
                    Name = step.Name,
                    Status = WorkflowStepStatus.ExtractionFailure,
                    ErrorMessage = $"Failed to extract variable '{extraction.VariableName}' from response."
                };
                result.Steps.Add(failedStep);
                return false;
            }
        }

        return true;
    }

    private static JsonNode? GetJsonPathValue(JsonNode? node, string path)
    {
        if (node == null || string.IsNullOrWhiteSpace(path))
            return null;

        var parts = path.Split('.');

        JsonNode? current = node;
        foreach (var part in parts)
        {
            if (current is JsonObject obj && obj.TryGetPropertyValue(part, out var next))
            {
                current = next;
            }
            else if (current is JsonArray arr && int.TryParse(part, out var index) && index < arr.Count)
            {
                current = arr[index];
            }
            else
            {
                return null;
            }
        }

        return current;
    }
    
    private static void SetJsonPathValue(JsonNode node, string path, JsonNode value)
    {
        var parts = path.Split('.');
        JsonNode current = node;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            var part = parts[i];
            if (current is JsonObject obj)
            {
                if (!obj.TryGetPropertyValue(part, out var next))
                {
                    next = new JsonObject();
                    obj[part] = next;
                }
                current = next;
            }
            else
            {
                throw new InvalidOperationException($"Invalid path '{path}' for JSON node.");
            }
        }

        if (current is JsonObject obj2)
        {
            obj2[parts[^1]] = value;
        }
    }
}