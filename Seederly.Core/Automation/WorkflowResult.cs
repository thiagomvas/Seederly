namespace Seederly.Core.Automation;

public class WorkflowResult
{
    public string WorkflowName { get; set; } = string.Empty;
    public string WorkflowDescription { get; set; } = string.Empty;
    public List<WorkflowStepResult> Steps { get; set; } = new();
    public bool IsSuccessful => Steps.All(step => step.Status == WorkflowStepStatus.Success);
    public string ErrorMessage { get; set; } = string.Empty;
    public long TotalElapsedMilliseconds => Steps.Sum(step => step.ElapsedMilliseconds);
    
}