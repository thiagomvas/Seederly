namespace Seederly.Core;

public class ApiRequest
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string Url { get; set; } = string.Empty;
    public string? Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? ContentType { get; set; } = "application/json";
    public ApiResponse? LastResponse { get; set; } = null;
    public ApiRequest WithJwt(string jwt)
    {
        Headers["Authorization"] = $"Bearer {jwt}";
        return this;
    }
}