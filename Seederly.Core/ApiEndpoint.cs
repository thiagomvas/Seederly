namespace Seederly.Core;

public class ApiEndpoint
{
    public string Name { get; set; } = string.Empty;
    public ApiRequest Request { get; set; } = new();
    public Dictionary<string, string> Schema { get; set; } = new();
    
    public List<ApiEndpoint> Children { get; set; } = new();
    
    public ApiEndpoint()
    {
    }
    
    
    public ApiEndpoint(string name, ApiRequest request)
    {
        Name = name;
        Request = request;
    }
}