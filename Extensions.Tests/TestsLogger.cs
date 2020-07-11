using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extensions.Tests
{
    public class TestsLogger : ILogger
    {
        private readonly TestContext _context;
        private readonly string _name;

        public TestsLogger(TestContext context, string name)
        {
            _context = context;
            _name = name;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            string log = $"[{DateTime.Now}] [{logLevel.ToString()}] [{_name}] {message}";
            
            _context.WriteLine(log);
            Console.WriteLine(log);
            Debug.WriteLine(log);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => new EmptyDisposable();
    }
}