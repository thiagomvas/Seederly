using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.ViewModels;

public partial class WorkflowViewModel : ViewModelBase
{

    public ObservableCollection<WorkflowModel> Workflows { get; } = new();
    
    [ObservableProperty] private WorkflowModel _selectedWorkflow;

    private readonly WorkflowExecutor _executor;
    private readonly VariableContext _variableContext;
    private readonly ApiRequestExecutor _apiRequestExecutor;
    private readonly Workspace _workspace;

    public WorkflowViewModel(Workspace workspace)
    {
        _variableContext = new VariableContext();
        _apiRequestExecutor = new ApiRequestExecutor(new HttpClient());
        _executor = new WorkflowExecutor(_apiRequestExecutor, _variableContext, workspace);
        _workspace = workspace;
        
        foreach (var workflow in workspace.Workflows)
        {
            var model = WorkflowModel.FromWorkflow(workflow);
            Workflows.Add(model);
            model.PropertyChanged += OnAnyPropertyChanged;
        }
        
        Workflows.CollectionChanged += OnCollectionChanged;
    }
    
    [RelayCommand]
    private void AddWorkflow()
    {
        var newWorkflow = new WorkflowModel()
        {
            Name = "New workflow",
            Steps = new ObservableCollection<WorkflowStepModel>(),
        };
        
        Workflows.Add(newWorkflow);
        SelectedWorkflow = newWorkflow;
    }
    
    private void RemoveWorkflow(WorkflowModel workflow)
    {
        if (workflow == null)
            return;

        Workflows.Remove(workflow);
    }
    
    private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (WorkflowModel workflowModel in e.NewItems)
            {
                workflowModel.PropertyChanged += OnAnyPropertyChanged;
                workflowModel.Steps.CollectionChanged += OnCollectionChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (WorkflowModel workflowModel in e.OldItems)
            {
                workflowModel.PropertyChanged -= OnAnyPropertyChanged;
                workflowModel.Steps.CollectionChanged -= OnCollectionChanged;
            }
        }
        
        SaveWorkspace();
    }

    private void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        SaveWorkspace();
    }

    private void SaveWorkspace()
    {
        // Convert the workflowmodels into workflows
        var workflows = new List<Workflow>();
        foreach (var workflowModel in Workflows)
        {
            var workflow = new Workflow()
            {
                Name = workflowModel.Name,
                Steps = new List<WorkflowStep>(),
                Description = workflowModel.Description
            };
            foreach (var stepModel in workflowModel.Steps)
            {
                var step = new WorkflowStep()
                {
                    Name = stepModel.Name,
                    Extract = new(),
                    Inject = new(),
                    Description = stepModel.Description,
                };
                
                foreach (var variable in stepModel.Extract)
                {
                    step.Extract.Add(new VariableExtractionRule()
                    {
                        VariableName = variable.VariableName,
                        Source = variable.Source,
                        JsonPath = variable.JsonPath
                    });
                }
                
                foreach (var variable in stepModel.Inject)
                {
                    step.Inject.Add(new VariableInjectionRule()
                    {
                        Key = variable.Key,
                        Path = variable.Path,
                        Target = variable.Target
                    });
                }
                workflow.Steps.Add(step);
            }
            workflows.Add(workflow);
        }
        
        // Save the workflows to the workspace
        _workspace.Workflows.Clear();
        foreach (var workflow in workflows)
        {
            _workspace.Workflows.Add(workflow);
        }
        
        // Save the workspace to the file
        if (!string.IsNullOrWhiteSpace(_workspace.Path))
        {
            File.WriteAllText(_workspace.Path, JsonSerializer.Serialize(_workspace, new JsonSerializerOptions() { WriteIndented = true }));
        }
        
    }
}