using System.IO;
using System.Text.Json;
using Seederly.Core;

namespace Seederly.Desktop;

public class Utils
{
    public static void SaveWorkspace(Workspace workspace)
    {
        if (workspace == null)
            return;

        if (string.IsNullOrWhiteSpace(workspace.Path))
            return;
        
        
        var filePath = workspace.Path;
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);
        var json = workspace.SerializeToJson();
        File.WriteAllText(filePath, json);
    }
}