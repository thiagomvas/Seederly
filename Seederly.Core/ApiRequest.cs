namespace Seederly.Core;

public class ApiRequest
{
    public string Name { get; set; } = string.Empty;
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string Url { get; set; } = string.Empty;
    public string? Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? ContentType { get; set; }
}