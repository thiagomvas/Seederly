using System;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Seederly.Desktop.Models;

public class LogEntry
{
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public ImmutableSolidColorBrush Color => Level switch
    {
        LogLevel.Debug => new ImmutableSolidColorBrush(Colors.Gray),
        LogLevel.Info => new ImmutableSolidColorBrush(Colors.DarkCyan),
        LogLevel.Warning => new ImmutableSolidColorBrush(Colors.Orange),
        LogLevel.Error => new ImmutableSolidColorBrush(Colors.Red),
        LogLevel.Critical => new ImmutableSolidColorBrush(Colors.Purple),
        _ => throw new ArgumentOutOfRangeException()
    };
}