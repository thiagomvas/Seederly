using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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
}