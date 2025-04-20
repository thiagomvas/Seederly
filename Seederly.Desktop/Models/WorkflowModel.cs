using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public partial class WorkflowModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ObservableCollection<WorkflowStepModel> Steps { get; set; } = new();

    public static WorkflowModel FromWorkflow(Workflow workflow)
    {
        var model = new WorkflowModel
        {
            Name = workflow.Name,
            Description = workflow.Description,
            Steps = new ObservableCollection<WorkflowStepModel>()
        };

        foreach (var step in workflow.Steps)
        {
            model.Steps.Add(WorkflowStepModel.FromWorkflowStep(step));
        }

        return model;
        
    }
}