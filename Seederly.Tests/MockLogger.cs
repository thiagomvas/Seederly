using Seederly.Core;

namespace Seederly.Tests;

public class MockLogger : ILogger
{
    public bool LoggedInformation { get; private set; }
    public bool LoggedError { get; private set; }
    public bool LoggedWarning { get; private set; }
    public bool LoggedDebug { get; private set; }
    public bool LoggedCritical { get; private set; }
    public void LogInformation(string message) => LoggedInformation = true;

    public void LogError(string message) => LoggedError = true;

    public void LogWarning(string message) => LoggedWarning = true;

    public void LogDebug(string message) => LoggedDebug = true;

    public void LogCritical(string message) => LoggedCritical = true;
}