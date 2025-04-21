using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public class VariableExtractionRuleModel : ObservableObject
{
    public string VariableName { get; set; } = "";
    public string JsonPath { get; set; } = ""; 
    public ExtractionVariableTarget Source { get; set; } = ExtractionVariableTarget.Response;
    public WorkflowStepModel Parent { get; set; } 
}