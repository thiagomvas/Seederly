using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Seederly.Core;

namespace Seederly.Desktop.Services;

public class SessionService
{
    private static SessionService _instance;
    public static SessionService Instance => _instance ??= new SessionService();
    public SessionData Data { get; }
    public Workspace LoadedWorkspace { get; set; } = new Workspace("New Workspace");

    private readonly string _appDataPath;

    private SessionService()
    {
        _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seederly");
        
        if (!Directory.Exists(_appDataPath))
        {
            Directory.CreateDirectory(_appDataPath);
        }
        
        var path = Path.Combine(_appDataPath, "session.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            Data = JsonSerializer.Deserialize<SessionData>(json) ?? new SessionData();
        }
        else
        {
            Data = new SessionData();
        }
    }
    
    public Workspace GetLastOpenedWorkspace()
    {
        if (string.IsNullOrWhiteSpace(Data.LastWorkspacePath) || !File.Exists(Data.LastWorkspacePath))
        {
            return new Workspace("New Workspace");
        }

        var json = File.ReadAllText(Data.LastWorkspacePath);
        return Workspace.DeserializeFromJson(json);
    }

    public void SaveData()
    {
        var json = JsonSerializer.Serialize(Data);
        var path = Path.Combine(_appDataPath, "session.json");
        
        File.WriteAllText(path, json);
    }

    public void SaveWorkspace()
    {
        if (LoadedWorkspace == null)
        {
            throw new InvalidOperationException("No workspace loaded to save.");
        }

        var json = LoadedWorkspace.SerializeToJson();
        
        File.WriteAllText(LoadedWorkspace.Path, json);
        
        // Update the last workspace path
        Data.LastWorkspacePath = LoadedWorkspace.Path;
        SaveData();
    }
}