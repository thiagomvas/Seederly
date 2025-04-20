using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public partial class WorkflowModel : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;

    [ObservableProperty] private bool _isEditing;
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