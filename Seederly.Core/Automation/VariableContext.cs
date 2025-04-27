namespace Seederly.Core.Automation;

/// <summary>
/// Represents a context for storing and retrieving variables.
/// </summary>
public class VariableContext
{
    private readonly Dictionary<string, string> _variables = new(StringComparer.OrdinalIgnoreCase);
    
    /// <summary>
    /// Gets or sets a variable by its key.
    /// </summary>
    /// <param name="key">The key to get or set the value for.</param>
    public string this[string key]
    {
        get => _variables.TryGetValue(key, out var value) ? value : string.Empty;
        set => _variables[key] = value;
    }
    
    /// <summary>
    /// Clears all variables from the context.
    /// </summary>
    public void Clear()
    {
        _variables.Clear();
    }
}