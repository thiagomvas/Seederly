namespace Seederly.Core.Codegen;

public class CurlCodeGenerator : ICodeGen
{
    public string GenerateCode(ApiRequest request)
    {
        var curlCommand = $"curl -X {request.Method} '{request.BuildRoute()}'";

        // Add headers
        foreach (var header in request.Headers)
        {
            curlCommand += $"\\\n -H '{header.Key}: {header.Value}'";
        }
        
        // Add Content-Type header if specified
        if (!string.IsNullOrEmpty(request.ContentType))
        {
            curlCommand += $"\\\n -H 'Content-Type: {request.ContentType}'";
        }

        // Add body if present
        if (!string.IsNullOrEmpty(request.Body))
        {
            curlCommand += $"\\\n -d '{request.Body}'";
        }

        return curlCommand;
    }
}