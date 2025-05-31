namespace Seederly.Core.Codegen;

public class CurlCodeGenerator : ICodeGen
{
    public string GenerateCode(ApiRequest request)
    {
        var curlCommand = $"curl -X {request.Method} '{request.BuildRoute()}'";

        // Add headers
        foreach (var header in request.GetHeaders())
        {
            curlCommand += $"\\\n -H '{header.Key}: {header.Value}'";
        }

        // Add body if present
        if (!string.IsNullOrEmpty(request.Body))
        {
            curlCommand += $"\\\n -d '{request.Body}'";
        }

        return curlCommand;
    }
}