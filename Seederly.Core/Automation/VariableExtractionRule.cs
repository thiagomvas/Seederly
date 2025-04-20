namespace Seederly.Core.Automation;

public class VariableExtractionRule
{
    public string VariableName { get; set; } = "";
    public string JsonPath { get; set; } = ""; 
    public VariableTarget Source { get; set; } = VariableTarget.None;
}

