namespace Seederly.Core.Automation;

public class Workflow
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WorkflowStep> Steps { get; set; } = new();
}