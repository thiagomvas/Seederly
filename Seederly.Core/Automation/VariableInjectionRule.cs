namespace Seederly.Core.Automation;

public class VariableInjectionRule
{
    public VariableTarget Target { get; set; } = VariableTarget.Header; 
    public string Key { get; set; } = "";
    public string Path { get; set; } = ""; 
}
