namespace Seederly.Core.Automation;

public class WorkflowStep
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EndpointName { get; set; } = string.Empty;
    public List<VariableExtractionRule> Extract { get; set; } = new();
    public List<VariableInjectionRule> Inject { get; set; } = new();
    
}