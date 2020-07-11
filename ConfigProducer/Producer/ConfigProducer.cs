using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConfigProducer
{
    internal class ConfigProducer : BackgroundService
    {
        private readonly BaseKafkaConfig _config;
        private readonly ConfigReader _reader;
        private readonly IKafkaProducer<string, string> _producer;
        private readonly ILogger<ConfigProducer> _logger;
        
        public ConfigProducer(
            BaseKafkaConfig config,
            ConfigReader reader,
            IKafkaProducer<string, string> producer,
            ILoggerFactory loggerFactory)
        {
            _config = config;
            _reader = reader;
            _producer = producer;

            _logger = loggerFactory.CreateLogger<ConfigProducer>();
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sending configs");
            
            foreach ((string fileName, string contents) in _reader.ReadAllConfigs())
            {
                _logger.LogInformation("Sending {} : {}", fileName, contents);
                _producer.Produce(fileName, contents);
            }
            
            _logger.LogInformation("Done sending configs");

            return Task.CompletedTask;
        }
    }
}