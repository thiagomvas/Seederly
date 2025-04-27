namespace Seederly.Core.Automation;

/// <summary>
/// Represents the result of executing a single workflow step.
/// </summary>
public class WorkflowStepResult
{
    /// <summary>
    /// Gets or sets the name of the executed step.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution status of the step.
    /// </summary>
    public WorkflowStepStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the error message if the step execution failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the API response received during the step execution.
    /// </summary>
    public ApiResponse? Response { get; set; }

    /// <summary>
    /// Gets or sets the elapsed time for the step execution in milliseconds.
    /// </summary>
    public long ElapsedMilliseconds { get; set; }
}
