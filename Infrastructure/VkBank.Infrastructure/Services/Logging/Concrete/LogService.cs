﻿using Microsoft.Extensions.Logging;
using VkBank.Infrastructure.Services.Logging.Abstract;

namespace VkBank.Infrastructure.Services.Logging.Concrete
{
    public class LogService<T> : ILogService<T>
    {
        private readonly ILogger<T> _logger;

        public LogService(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        public void LogError(string message)
        {
            _logger.LogError(message);
        }

        public void LogError(Exception exception, string message)
        {
            _logger.LogError(exception, message);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void LogTrace(string message)
        {
            _logger.LogTrace(message);
        }

        public void LogCritical(string message)
        {
            _logger.LogCritical(message);
        }

        public void LogCritical(Exception exception, string message)
        {
            _logger.LogCritical(exception, message);
        }
    }
}
