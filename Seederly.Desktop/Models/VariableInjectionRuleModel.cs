using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public partial class VariableInjectionRuleModel : ObservableObject
{
    [ObservableProperty] private string _key = "";
    [ObservableProperty] private string _path = "";
    [ObservableProperty] private InjectionVariableTarget _target = InjectionVariableTarget.Body;
    [ObservableProperty] private string _targetString = "";
    [ObservableProperty] private int _selectedIndex;
    public WorkflowStepModel? Parent { get; set; }
    
}