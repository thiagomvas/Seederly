using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Seederly.Core;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.Services;

public partial class LoggerService : ObservableObject
{
    #region Singleton

    private static readonly Lazy<LoggerService> _instance = new(() => new LoggerService());
    public static LoggerService Instance => _instance.Value;
    
    private LoggerService()
    {
        // Private constructor to prevent instantiation
        LogEntries.CollectionChanged += (sender, args) =>
        {
            if (args.NewItems == null) return;
            foreach (LogEntry logEntry in args.NewItems)
            {
                LogText += $"{logEntry.Timestamp}: {logEntry.Message}\n";
            }
        };
        Log("Logger Service initialized.");
    }
    #endregion
    
    public ObservableCollection<LogEntry> LogEntries { get; } = new();
    [ObservableProperty] private string _logText = string.Empty;
    public void Log(string message)
    {
        var logEntry = new LogEntry
        {
            Message = message,
            Timestamp = DateTime.Now
        };
        
        LogEntries.Add(logEntry);
    }
    
    public void Log(ApiRequest request)
    {
        var lines = new List<string>
        {
            "----- API Request -----",
            $"Method: {request.Method}",
            $"URL: {request.BuildRoute()}"
        };

        if (request.Headers.Count > 0)
        {
            lines.Add("Headers:");
            foreach (var header in request.Headers)
            {
                lines.Add($"  {header.Key}: {header.Value}");
            }
        }

        if (!string.IsNullOrEmpty(request.Body))
        {
            lines.Add("Body:");
            lines.Add(request.Body);
        }

        lines.Add("------------------------");

        Log(string.Join(Environment.NewLine, lines));
    }


    public void Log(ApiResponse response)
    {
        var lines = new List<string>
        {
            "----- API Response -----",
            $"Status Code: {(int)response.StatusCode} ({response.StatusCode})",
            "Headers:"
        };

        foreach (var header in response.Headers)
        {
            lines.Add($"  {header.Key}: {header.Value}");
        }

        lines.Add("Content:");
        lines.Add(response.Content);
        lines.Add("-------------------------");

        Log(string.Join(Environment.NewLine, lines));
    }

}