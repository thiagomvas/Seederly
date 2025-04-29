using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.Services;

public partial class LoggerService : ObservableObject, ILogger
{
    #region Singleton

    private static readonly Lazy<LoggerService> _instance = new(() => new LoggerService());
    public static LoggerService Instance => _instance.Value;
    
    private LoggerService()
    {
    }
    #endregion
    
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    public void Log(string message, LogLevel level = LogLevel.Info)
    {
        var logEntry = new LogEntry
        {
            Message = message,
            Timestamp = DateTime.Now,
            Level = level
        };
        
        LogEntries.Add(logEntry);
    }
    
    public void Log(ApiRequest request)
    {
        var lines = new List<string>
        {
            $"[Request] {request.Method} {request.BuildRoute()}",
            $"Headers ({request.Headers.Count}):"
        };

        foreach (var header in request.Headers)
        {
            lines.Add($"  {header.Key}: {header.Value}");
        }

        if (!string.IsNullOrWhiteSpace(request.Body))
        {
            lines.Add("Body:");
            lines.Add(IndentJsonIfPossible(request.Body));
        }

        var message = string.Join(Environment.NewLine, lines);
        LogEntries.Add(new LogEntry
        {
            Message = message,
            Timestamp = DateTime.Now,
            Level = LogLevel.Debug
        });
    }



    public void Log(ApiResponse response)
    {
        var lines = new List<string>
        {
            $"[Response] {(int)response.StatusCode} {response.StatusCode}",
            $"Headers ({response.Headers.Count}):"
        };

        foreach (var header in response.Headers)
        {
            lines.Add($"  {header.Key}: {header.Value}");
        }

        lines.Add("Content:");
        lines.Add(IndentJsonIfPossible(response.Content));

        var message = string.Join(Environment.NewLine, lines);
        LogEntries.Add(new LogEntry
        {
            Message = message,
            Timestamp = DateTime.Now,
            Level = LogLevel.Debug
        });
    }

    
    private string IndentJsonIfPossible(string json)
    {
        try
        {
            var parsed = System.Text.Json.JsonDocument.Parse(json);
            return System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        catch
        {
            return json; // Fallback to raw if not valid JSON
        }
    }


    public void LogInformation(string message)
    {
        Log(message, LogLevel.Info);
    }

    public void LogError(string message)
    {
        Log(message, LogLevel.Error);
    }

    public void LogWarning(string message)
    {
        Log(message, LogLevel.Warning);
    }

    public void LogDebug(string message)
    {
        Log(message, LogLevel.Debug);
    }

    public void LogCritical(string message)
    {
        Log(message, LogLevel.Critical);
    }
}