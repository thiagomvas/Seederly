namespace Seederly.Core.Automation;

/// <summary>
/// Represents a workflow responsible for running sequential api calls with injection and extraction operations.
/// </summary>
public class Workflow
{
    /// <summary>
    /// Gets or sets the name of the workflow.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the workflow.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of steps in the workflow.
    /// </summary>
    public List<WorkflowStep> Steps { get; set; } = new();
}
