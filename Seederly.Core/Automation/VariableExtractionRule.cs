namespace Seederly.Core.Automation;

/// <summary>
/// Represents a rule for extracting a variable from a response.
/// </summary>
public class VariableExtractionRule
{
    /// <summary>
    /// Gets or sets the name of the variable to extract.
    /// </summary>
    public string VariableName { get; set; } = "";

    /// <summary>
    /// Gets or sets the JSON path expression used to locate the value.
    /// </summary>
    public string JsonPath { get; set; } = "";

    /// <summary>
    /// Gets or sets the source from which the variable will be extracted.
    /// </summary>
    public ExtractionVariableTarget Source { get; set; } = ExtractionVariableTarget.Response;
}

