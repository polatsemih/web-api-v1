namespace SUPBank.Application.Interfaces.Services
{
    public interface ILogService<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(string message, Exception exception);
        void LogDebug(string message);
        void LogTrace(string message);
        void LogCritical(string message);
        void LogCritical(string message, Exception exception);
    }
}
