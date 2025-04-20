using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core.Automation;

namespace Seederly.Desktop.Models;

public class VariableInjectionRuleModel : ObservableObject
{
    
    public InjectionVariableTarget Target { get; set; } = InjectionVariableTarget.Body; 
    public string Key { get; set; } = "";
    public string Path { get; set; } = ""; 
    
}