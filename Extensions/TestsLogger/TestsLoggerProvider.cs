using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extensions
{
    public class TestsLoggerProvider : ILoggerProvider
    {
        private readonly TestContext _context;
        private readonly ConcurrentDictionary<string, TestsLogger> _loggers;

        public TestsLoggerProvider(TestContext context)
        {
            _context = context;
            _loggers = new ConcurrentDictionary<string, TestsLogger>();
        }

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(
            categoryName,
            name => new TestsLogger(_context, name));

        public void Dispose() => _loggers.Clear();
    }
}