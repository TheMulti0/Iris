using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConfigProducer
{
    internal class ConfigProducer : BackgroundService
    {
        private readonly KafkaConfig _config;
        private readonly ConfigReader _reader;
        private readonly Producer<string, string> _producer;
        private readonly ILogger<ConfigProducer> _logger;
        
        public ConfigProducer(
            KafkaConfig config,
            ConfigReader reader,
            ILoggerFactory loggerFactory)
        {
            _config = config;
            _reader = reader;
            _producer = new Producer<string, string>(
                config,
                loggerFactory);
            
            _logger = loggerFactory.CreateLogger<ConfigProducer>();
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sending configs");
            
            foreach ((string fileName, string contents) in _reader.ReadAllConfigs())
            {
                _producer.Produce(_config.ConfigsTopic, fileName, contents);
            }
            
            _logger.LogInformation("Done sending configs");

            return Task.CompletedTask;
        }
    }
}