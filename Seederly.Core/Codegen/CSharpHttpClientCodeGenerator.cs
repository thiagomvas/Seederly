using System.Text;

namespace Seederly.Core.Codegen;

public class CSharpHttpClientCodeGenerator : ICodeGen
{
    public string GenerateCode(ApiRequest request)
    {
        var code = new StringBuilder();
        
        code.AppendLine($"var url = \"{request.Url}\";");
        if(!string.IsNullOrWhiteSpace(request.Body))
        {
            code.AppendLine($"var body = new StringContent(\"{request.Body.Replace("\"", "\\\"").ReplaceLineEndings("")}\", System.Text.Encoding.UTF8, \"application/json\");");
        }
        
        code.AppendLine($"using var client = new HttpClient();");
        
        // Add headers
        if (request.Headers.Count != 0)
        {
            foreach (var header in request.Headers)
            {
                code.AppendLine($"client.DefaultRequestHeaders.Add(\"{header.Key}\", \"{header.Value}\");");
            }
        }
        
        code.AppendLine($"var response = await client.{GetHttpClientMethod(request.Method)};");
        code.AppendLine("if (response.IsSuccessStatusCode)");
        code.AppendLine("{");
        code.AppendLine("    var content = await response.Content.ReadAsStringAsync();");
        code.AppendLine("    Console.WriteLine(content);");
        code.AppendLine("}");
        code.AppendLine("else");
        code.AppendLine("{");
        code.AppendLine("    Console.WriteLine($\"Error: {response.StatusCode} - {response.ReasonPhrase}\");");
        code.AppendLine("}");

        return code.ToString();
    }

    private string GetHttpClientMethod(HttpMethod method)
    {
        if (method == HttpMethod.Get)
        {
            return "GetAsync(url)";
        }
        else if (method == HttpMethod.Post)
        {
            return "PostAsync(url, body)";
        }
        else if (method == HttpMethod.Put)
        {
            return "PutAsync(url, body)";
        }
        else if (method == HttpMethod.Delete)
        {
            return "DeleteAsync(url)";
        }
        else if (method == HttpMethod.Patch)
        {
            return "PatchAsync(url, body)";
        }
        else
        {
            throw new NotSupportedException($"HTTP method {method} is not supported.");
        }
    }
}