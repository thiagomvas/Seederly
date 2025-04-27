using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public partial class WorkflowStepModel : ObservableObject
{
    [ObservableProperty] private string _name;
    [ObservableProperty] private string _description;
    [ObservableProperty] private string _endpointName;
    [ObservableProperty] private bool _isEditing;
    
    [ObservableProperty] private string _method = "";
    [ObservableProperty] private string _url = "";

    public string FormattedUrl => $"Endpoint: {Method} {Url}";
    partial void OnMethodChanged(string value) => OnPropertyChanged(nameof(FormattedUrl));
    partial void OnUrlChanged(string value) => OnPropertyChanged(nameof(FormattedUrl));

    public ObservableCollection<VariableExtractionRuleModel> Extract { get; set; } = new();
    public ObservableCollection<VariableInjectionRuleModel> Inject { get; set; } = new();

    public static WorkflowStepModel FromWorkflowStep(WorkflowStep step)
    {
        var model = new WorkflowStepModel
        {
            Name = step.Name,
            Description = step.Description,
            EndpointName = step.EndpointName,
            Extract = new ObservableCollection<VariableExtractionRuleModel>(),
            Inject = new ObservableCollection<VariableInjectionRuleModel>()
        };

        foreach (var extract in step.Extract)
        {
            model.Extract.Add(new VariableExtractionRuleModel
            {
                VariableName = extract.VariableName,
                JsonPath = extract.JsonPath,
                SelectedIndex = (int)extract.Source
            });
        }

        foreach (var inject in step.Inject)
        {
            model.Inject.Add(new VariableInjectionRuleModel
            {
                Target = inject.Target,
                Key = inject.Key,
                Path = inject.Path,
                SelectedIndex = (int)inject.Target,
            });
        }

        return model;
    }
    
    [RelayCommand]
    private void ToggleEdit() => IsEditing = !IsEditing;
    
    [RelayCommand]
    private void AddExtractRule()
    {
        var newRule = new VariableExtractionRuleModel
        {
            VariableName = "New variable",
            JsonPath = "$.data",
            Parent = this,
            SelectedIndex = 0,
            Source = ExtractionVariableTarget.Response,
        };
        
        Extract.Add(newRule);
    }
    [RelayCommand]
    private void RemoveExtractRule(VariableExtractionRuleModel rule)
    {
        if (rule != null)
        {
            Extract.Remove(rule);
        }
    }
    
    [RelayCommand]
    private void AddInjectRule()
    {
        var newRule = new VariableInjectionRuleModel
        {
            TargetString = InjectionVariableTarget.Body.ToString(),
            Key = "New key",
            Path = "$.data",
            Parent = this,
            Target = InjectionVariableTarget.Body,
            SelectedIndex = 0
        };
        
        Inject.Add(newRule);
    }
    [RelayCommand]
    private void RemoveInjectRule(VariableInjectionRuleModel rule)
    {
        if (rule != null)
        {
            Inject.Remove(rule);
        }
    }

    public WorkflowStep ToWorkflowStep()
    {
        var step = new WorkflowStep
        {
            Name = Name,
            Description = Description,
            EndpointName = EndpointName,
            Extract = new List<VariableExtractionRule>(),
            Inject = new List<VariableInjectionRule>()
        };

        foreach (var extract in Extract)
        {
            step.Extract.Add(new VariableExtractionRule
            {
                VariableName = extract.VariableName,
                JsonPath = extract.JsonPath,
                Source = (ExtractionVariableTarget)extract.SelectedIndex
            });
        }

        foreach (var inject in Inject)
        {
            step.Inject.Add(new VariableInjectionRule
            {
                Target = inject.Target,
                Key = inject.Key,
                Path = inject.Path,
            });
        }

        return step;
    }
}