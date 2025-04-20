namespace Seederly.Core.Automation;

public class VariableContext
{
    private readonly Dictionary<string, string> _variables = new(StringComparer.OrdinalIgnoreCase);
    
    public string this[string key]
    {
        get => _variables.TryGetValue(key, out var value) ? value : string.Empty;
        set => _variables[key] = value;
    }
    
    public void Clear()
    {
        _variables.Clear();
    }
}