namespace Seederly.Core.Automation;

public class VariableInjectionRule
{
    public InjectionVariableTarget Target { get; set; } = InjectionVariableTarget.Body; 
    public string Key { get; set; } = "";
    public string Path { get; set; } = ""; 
}
