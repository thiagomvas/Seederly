using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public partial class VariableExtractionRuleModel : ObservableObject
{
    [ObservableProperty] private string _variableName;
    [ObservableProperty] private string _jsonPath;
    [ObservableProperty] private ExtractionVariableTarget _source;
    [ObservableProperty] private string _target;
    [ObservableProperty] private int _selectedIndex;
    public WorkflowStepModel Parent { get; set; } 
}