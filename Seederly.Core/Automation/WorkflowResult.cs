namespace Seederly.Core.Automation;

/// <summary>
/// Represents the result of executing a workflow.
/// </summary>
public class WorkflowResult
{
    /// <summary>
    /// Gets or sets the name of the executed workflow.
    /// </summary>
    public string WorkflowName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the executed workflow.
    /// </summary>
    public string WorkflowDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of results for each workflow step.
    /// </summary>
    public List<WorkflowStepResult> Steps { get; set; } = new();

    /// <summary>
    /// Gets a value indicating whether the workflow execution was successful.
    /// </summary>
    public bool IsSuccessful => Steps.All(step => step.Status == WorkflowStepStatus.Success);

    /// <summary>
    /// Gets or sets the error message if the workflow failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total elapsed time for the workflow execution in milliseconds.
    /// </summary>
    public long TotalElapsedMilliseconds => Steps.Sum(step => step.ElapsedMilliseconds);
}
