namespace Seederly.Core.Configuration;

public class StagingEnvironment
{
    public string Name { get; set; } = "Staging";
    public string BaseUrl { get; set; } = "https://staging.api.seederly.com";
    public string DocumentationUrl { get; set; } = "https://staging.api.seederly.com";
    
    public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
}