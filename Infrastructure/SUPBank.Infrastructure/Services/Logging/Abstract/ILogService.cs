namespace SUPBank.Infrastructure.Services.Logging.Abstract
{
    public interface ILogService<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception exception, string message);
        void LogDebug(string message);
        void LogTrace(string message);
        void LogCritical(string message);
        void LogCritical(Exception exception, string message);
    }
}
