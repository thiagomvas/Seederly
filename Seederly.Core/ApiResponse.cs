using System.Net;

namespace Seederly.Core;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 300;
}