namespace Seederly.Core.Automation;

/// <summary>
/// Represents a rule for injecting a variable into a request.
/// </summary>
public class VariableInjectionRule
{
    /// <summary>
    /// Gets or sets the target where the variable will be injected.
    /// </summary>
    public InjectionVariableTarget Target { get; set; } = InjectionVariableTarget.Body;

    /// <summary>
    /// Gets or sets the key associated with the injected variable.
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Gets or sets the path where the variable should be injected.
    /// </summary>
    public string Path { get; set; } = "";
}

