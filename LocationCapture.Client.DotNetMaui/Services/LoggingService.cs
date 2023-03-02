using LocationCapture.BL;
using Microsoft.Extensions.Logging;

namespace LocationCapture.Client.DotNetMaui.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            _logger.LogDebug(messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.LogError(new EventId(-1), exception, messageTemplate, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.LogCritical(new EventId(-2), exception, messageTemplate, propertyValues);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            _logger.LogInformation(messageTemplate, propertyValues);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            _logger.LogTrace(messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            _logger.LogWarning(messageTemplate, propertyValues);
        }
    }
}
