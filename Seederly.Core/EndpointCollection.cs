namespace Seederly.Core;

public class EndpointCollection
{
    public string Name { get; set; } = string.Empty;
    
    public List<EndpointCollection> Children { get; set; } = new();
    public List<ApiRequest> Endpoints { get; set; } = new();
}