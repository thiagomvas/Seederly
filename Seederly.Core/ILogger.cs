namespace Seederly.Core;

public interface ILogger
{
    void LogInformation(string message);
    void LogError(string message);
    void LogWarning(string message);
    void LogDebug(string message);
    void LogCritical(string message);
}