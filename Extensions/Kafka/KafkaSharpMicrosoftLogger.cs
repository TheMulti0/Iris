using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class KafkaSharpMicrosoftLogger : Kafka.Public.ILogger
    {
        private readonly ILogger _logger;

        public KafkaSharpMicrosoftLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
            => _logger.LogInformation(message);

        public void LogWarning(string message)
            => _logger.LogWarning(message);

        public void LogError(string message)
            => _logger.LogError(message);

        public void LogDebug(string message)
            => _logger.LogDebug(message);
    }
}