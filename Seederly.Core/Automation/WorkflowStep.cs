namespace Seederly.Core.Automation;

/// <summary>
/// Represents a single step within a workflow.
/// </summary>
public class WorkflowStep
{
    /// <summary>
    /// Gets or sets the name of the step.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the step.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the endpoint associated with this step.
    /// </summary>
    public string EndpointName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of variable extraction rules for this step.
    /// </summary>
    public List<VariableExtractionRule> Extract { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of variable injection rules for this step.
    /// </summary>
    public List<VariableInjectionRule> Inject { get; set; } = new();
}
