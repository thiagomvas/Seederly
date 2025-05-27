using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Seederly.Desktop.Services;

public class SessionService
{
    private static SessionService _instance;
    public static SessionService Instance => _instance ??= new SessionService();
    public SessionData Data { get; }

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

    public void SaveData()
    {
        var json = JsonSerializer.Serialize(Data);
        var path = Path.Combine(_appDataPath, "session.json");
        
        File.WriteAllText(path, json);
    }
}