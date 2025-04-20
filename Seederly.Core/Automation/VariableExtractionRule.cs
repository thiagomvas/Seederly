namespace Seederly.Core.Automation;

public class VariableExtractionRule
{
    public string VariableName { get; set; } = "";
    public string JsonPath { get; set; } = ""; 
    public ExtractionVariableTarget Source { get; set; } = ExtractionVariableTarget.Response;
}

