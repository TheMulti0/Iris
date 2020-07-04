using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class CustomConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CustomConsoleLogger> _loggers =
            new ConcurrentDictionary<string, CustomConsoleLogger>();

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(
            categoryName,
            name => new CustomConsoleLogger(name));

        public void Dispose() => _loggers.Clear();
    }
}