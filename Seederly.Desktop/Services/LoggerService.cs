using System;
using System.Collections.Generic;
using Seederly.Desktop.Models;

namespace Seederly.Desktop.Services;

public class LoggerService
{
    #region Singleton

    private static readonly Lazy<LoggerService> _instance = new(() => new LoggerService());
    public static LoggerService Instance => _instance.Value;
    
    private LoggerService()
    {
        // Private constructor to prevent instantiation
    }
    #endregion
    
    private readonly List<LogEntry> _logEntries = new();
    public IReadOnlyList<LogEntry> LogEntries => _logEntries.AsReadOnly();
    
    public void Log(string message)
    {
        var logEntry = new LogEntry
        {
            Message = message,
            Timestamp = DateTime.Now
        };
        
        _logEntries.Add(logEntry);
    }
}