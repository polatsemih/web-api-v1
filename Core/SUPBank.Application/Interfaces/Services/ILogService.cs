namespace SUPBank.Application.Interfaces.Services
{
    /// <summary>
    /// Represents a logging service for logging different levels of messages.
    /// </summary>
    /// <typeparam name="T">The type of the category associated with the log messages.</typeparam>
    public interface ILogService<T>
    {
        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogError(string message);

        /// <summary>
        /// Logs an error message along with the exception details.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void LogError(string message, Exception exception);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogDebug(string message);

        /// <summary>
        /// Logs a trace message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogTrace(string message);

        /// <summary>
        /// Logs a critical message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogCritical(string message);

        /// <summary>
        /// Logs a critical message along with the exception details.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log.</param>
        void LogCritical(string message, Exception exception);
    }
}
