using System.Text;

namespace Seederly.Core.Codegen;

public class JsFetchCodeGen : ICodeGen
{
    public string GenerateCode(ApiRequest request)
    {
        var sb = new StringBuilder();

        sb.AppendLine("const url = new URL(\"" + request.Url + "\");");

        // Query parameters
        if (request.QueryParameters.Count != 0)
        {
            foreach (var param in request.QueryParameters)
            {
                sb.AppendLine($"url.searchParams.append(\"{param.Key}\", \"{param.Value}\");");
            }
        }

        sb.AppendLine();
        sb.AppendLine("const headers = {");

        if (request.Headers?.Any() == true)
        {
            foreach (var header in request.Headers)
            {
                sb.AppendLine($"  \"{header.Key}\": \"{header.Value}\",");
            }
        }

        sb.AppendLine("};");
        sb.AppendLine();

        string bodyLine = "null";
        if (!string.IsNullOrWhiteSpace(request.Body))
        {
            sb.AppendLine("const body = JSON.stringify(");
            sb.AppendLine(request.Body.Trim());
            sb.AppendLine(");");
            bodyLine = "body";
            sb.AppendLine();
        }

        sb.AppendLine("fetch(url, {");
        sb.AppendLine($"  method: \"{request.Method}\",");
        sb.AppendLine("  headers: headers,");
        sb.AppendLine($"  body: {bodyLine}");
        sb.AppendLine("})");
        sb.AppendLine(".then(response => response.json())");
        sb.AppendLine(".then(data => console.log(data))");
        sb.AppendLine(".catch(error => console.error(\"Error:\", error));");

        return sb.ToString();
    }
}