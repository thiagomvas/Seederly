using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.ViewModels;

public partial class WorkflowViewModel : ViewModelBase
{
    public ObservableCollection<WorkflowModel> Workflows { get; } = new();
    public ObservableCollection<string> AvailableEndpointNames => new(_workspace.Endpoints.Select(e => e.Name));

    [NotifyPropertyChangedFor(nameof(SelectedAny))]
    [ObservableProperty] private WorkflowModel? _selectedWorkflow;
    public bool SelectedAny => SelectedWorkflow != null;

    public ObservableCollection<string> ExtractionVariableTargetStrings =>
        new(Enum.GetValues<ExtractionVariableTarget>().Select(e => e.ToString()));

    public ObservableCollection<string> InjectionVariableTargetStrings =>
        new(Enum.GetValues<InjectionVariableTarget>().Select(e => e.ToString()));
    

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
            foreach (var step in model.Steps)
            {
                var target = workspace.Endpoints
                    .FirstOrDefault(e =>
                        string.Equals(e.Name, step.EndpointName, StringComparison.OrdinalIgnoreCase));
                if (target is not null)
                {
                    step.Method = target.Request.Method.ToString().ToUpper();
                    step.Url = target.Request.Url;
                }
                step.Extract.CollectionChanged += OnCollectionChanged;
                foreach (var extract in step.Extract)
                {
                    extract.Target = extract.Source.ToString();
                    extract.Parent = step;
                    extract.PropertyChanged += OnAnyPropertyChanged; 
                }
                
                step.Inject.CollectionChanged += OnCollectionChanged;
                foreach (var inject in step.Inject)
                {
                    inject.Parent = step;
                    inject.TargetString = inject.Target.ToString();
                    inject.PropertyChanged += OnAnyPropertyChanged;
                }
                
                step.PropertyChanged += OnStepPropertyChanged;
                step.PropertyChanged += OnAnyPropertyChanged;
            }
            model.Steps.CollectionChanged += OnCollectionChanged;
        }
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

    [RelayCommand]
    private void RemoveWorkflow(WorkflowModel workflow)
    {
        if (workflow == null)
            return;

        Workflows.Remove(workflow);
    }

    [RelayCommand]
    private void AddNewStep()
    {
        if (SelectedWorkflow == null)
            return;

        var newStep = new WorkflowStepModel
        {
            Name = "New step"
        };

        SelectedWorkflow.Steps.Add(newStep);
        newStep.Extract.CollectionChanged += OnCollectionChanged;
        foreach (var extract in newStep.Extract)
        {
            extract.PropertyChanged += OnAnyPropertyChanged;
        }
        newStep.Inject.CollectionChanged += OnCollectionChanged;
        foreach (var inject in newStep.Inject)
        {
            inject.PropertyChanged += OnAnyPropertyChanged;
        }
        newStep.PropertyChanged += OnStepPropertyChanged;
        newStep.PropertyChanged += OnAnyPropertyChanged;
    }
    
    [RelayCommand]
    private void RemoveStep(WorkflowStepModel step)
    {
        if (SelectedWorkflow == null)
            return;

        if (step == null)
            return;

        SelectedWorkflow.Steps.Remove(step);
    }
    
    [RelayCommand]
    private void AddNewExtract()
    {
        if (SelectedWorkflow == null || SelectedWorkflow.SelectedStep == null)
            return;

        var newExtract = new VariableExtractionRuleModel
        {
            VariableName = "New variable",
            Source = ExtractionVariableTarget.Response,
            JsonPath = "",
            Parent = SelectedWorkflow.SelectedStep,
            SelectedIndex = 0
        };

        SelectedWorkflow.SelectedStep.Extract.Add(newExtract);
        newExtract.PropertyChanged += OnAnyPropertyChanged;
    }
    
    [RelayCommand]
    private void RemoveExtract(VariableExtractionRuleModel extract)
    {
        if (SelectedWorkflow == null || SelectedWorkflow.SelectedStep == null)
            return;

        if (extract == null)
            return;

        SelectedWorkflow.SelectedStep.Extract.Remove(extract);
    }

    private void OnCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (WorkflowModel workflowModel in e.NewItems.OfType<WorkflowModel>())
            {
                workflowModel.PropertyChanged += OnAnyPropertyChanged;
                workflowModel.Steps.CollectionChanged += OnCollectionChanged;
                foreach (var stepModel in workflowModel.Steps)
                {
                    stepModel.Extract.CollectionChanged += OnCollectionChanged;
                    foreach (var extract in stepModel.Extract)
                    {
                        extract.PropertyChanged += OnAnyPropertyChanged;
                    }
                    
                    stepModel.Inject.CollectionChanged += OnCollectionChanged;
                    foreach (var inject in stepModel.Inject)
                    {
                        inject.PropertyChanged += OnAnyPropertyChanged;
                    }
                    
                    stepModel.PropertyChanged += OnStepPropertyChanged;
                    stepModel.PropertyChanged += OnAnyPropertyChanged;
                }
            }

            foreach (WorkflowStepModel stepModel in e.NewItems.OfType<WorkflowStepModel>())
            {
                stepModel.PropertyChanged += OnStepPropertyChanged;
                stepModel.PropertyChanged += OnAnyPropertyChanged;
                stepModel.Extract.CollectionChanged += OnCollectionChanged;
                foreach (var extract in stepModel.Extract)
                {
                    extract.PropertyChanged += OnAnyPropertyChanged;
                }
                stepModel.Inject.CollectionChanged += OnCollectionChanged;
                foreach (var inject in stepModel.Inject)
                {
                    inject.PropertyChanged += OnAnyPropertyChanged;
                }
            }
        }

        if (e.OldItems != null)
        {
            foreach (WorkflowModel workflowModel in e.OldItems.OfType<WorkflowModel>())
            {
                workflowModel.PropertyChanged -= OnAnyPropertyChanged;
                workflowModel.Steps.CollectionChanged -= OnCollectionChanged;
                foreach (var stepModel in workflowModel.Steps)
                {
                    stepModel.Extract.CollectionChanged -= OnCollectionChanged;
                    foreach (var extract in stepModel.Extract)
                    {
                        extract.PropertyChanged -= OnAnyPropertyChanged;
                    }
                    
                    stepModel.Inject.CollectionChanged -= OnCollectionChanged;
                    foreach (var inject in stepModel.Inject)
                    {
                        inject.PropertyChanged -= OnAnyPropertyChanged;
                    }
                    
                    stepModel.PropertyChanged -= OnStepPropertyChanged;
                    stepModel.PropertyChanged -= OnAnyPropertyChanged;
                }
            }

            foreach (WorkflowStepModel stepModel in e.OldItems.OfType<WorkflowStepModel>())
            {
                stepModel.PropertyChanged -= OnStepPropertyChanged;
                stepModel.PropertyChanged -= OnAnyPropertyChanged;
                stepModel.Extract.CollectionChanged -= OnCollectionChanged;
                foreach (var extract in stepModel.Extract)
                {
                    extract.PropertyChanged -= OnAnyPropertyChanged;
                }
                stepModel.Inject.CollectionChanged -= OnCollectionChanged;
                foreach (var inject in stepModel.Inject)
                {
                    inject.PropertyChanged -= OnAnyPropertyChanged;
                }
            }
        }

        SaveWorkspace();
    }

    private void OnStepPropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        if (sender is not null && sender is WorkflowStepModel stepModel)
        {
            if (e.PropertyName == nameof(stepModel.EndpointName))
            {
                var target = _workspace.Endpoints
                    .FirstOrDefault(e =>
                        string.Equals(e.Name, stepModel.EndpointName, StringComparison.OrdinalIgnoreCase));
                if (target is null) return;
                stepModel.Method = target.Request.Method.ToString().ToUpper();
                stepModel.Url = target.Request.Url;
            }
        }
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
                    EndpointName = stepModel.EndpointName,
                    Extract = new(),
                    Inject = new(),
                    Description = stepModel.Description,
                };

                foreach (var variable in stepModel.Extract)
                {
                    step.Extract.Add(new VariableExtractionRule()
                    {
                        VariableName = variable.VariableName,
                        Source = (ExtractionVariableTarget)variable.SelectedIndex,
                        JsonPath = variable.JsonPath
                    });
                }

                foreach (var variable in stepModel.Inject)
                {
                    step.Inject.Add(new VariableInjectionRule()
                    {
                        Key = variable.Key,
                        Path = variable.Path,
                        Target = (InjectionVariableTarget)variable.SelectedIndex
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

        Utils.SaveWorkspace(_workspace);
    }
    
    [RelayCommand]
    private async Task ExecuteWorkflow()
    {
        if (SelectedWorkflow == null)
            return;

        var workflow = SelectedWorkflow.ToWorkflow();
        var result = await _executor.ExecuteAsync(workflow);
    }
}