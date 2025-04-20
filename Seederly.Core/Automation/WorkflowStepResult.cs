namespace Seederly.Core.Automation;

public class WorkflowStepResult
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkflowStepStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public ApiResponse? Response { get; set; }
    public long ElapsedMilliseconds { get; set; }
    
}