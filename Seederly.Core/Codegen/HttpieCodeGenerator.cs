namespace Seederly.Core.Codegen;

public class HttpieCodeGenerator : ICodeGen
{
    public string GenerateCode(ApiRequest request)
    {
        var headers = string.Join(" ", request.GetHeaders().Select(kvp => $"\\\n {kvp.Key}:\"{kvp.Value}\""));

        var url = request.BuildRoute();
        var command = $"http {request.Method} {url} {headers}";
        
        // Add body if present
        if (!string.IsNullOrEmpty(request.Body))
        {
            command = $"echo -n '{request.Body}' | {command}";
        }

        return command;
    }
}