using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public class WorkflowStepModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EndpointName { get; set; } = string.Empty;
    public List<VariableExtractionRuleModel> Extract { get; set; } = new();
    public List<VariableInjectionRuleModel> Inject { get; set; } = new();

    public static WorkflowStepModel FromWorkflowStep(WorkflowStep step)
    {
        var model = new WorkflowStepModel
        {
            Name = step.Name,
            Description = step.Description,
            EndpointName = step.EndpointName,
            Extract = new List<VariableExtractionRuleModel>(),
            Inject = new List<VariableInjectionRuleModel>()
        };

        foreach (var extract in step.Extract)
        {
            model.Extract.Add(new VariableExtractionRuleModel
            {
                VariableName = extract.VariableName,
                JsonPath = extract.JsonPath,
                Source = extract.Source
            });
        }

        foreach (var inject in step.Inject)
        {
            model.Inject.Add(new VariableInjectionRuleModel
            {
                Target = inject.Target,
                Key = inject.Key,
                Path = inject.Path
            });
        }

        return model;
    }
}