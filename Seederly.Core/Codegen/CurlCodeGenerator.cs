namespace Seederly.Core.Codegen;

public class CurlCodeGenerator : ICodeGen
{
    public string GenerateCode(ApiRequest request)
    {
        var curlCommand = $"curl -X {request.Method} '{request.BuildRoute()}'";

        // Add headers
        foreach (var header in request.Headers)
        {
            curlCommand += $" -H '{header.Key}: {header.Value}'";
        }

        // Add body if present
        if (!string.IsNullOrEmpty(request.Body))
        {
            curlCommand += $" -d '{request.Body}'";
        }

        return curlCommand;
    }
}