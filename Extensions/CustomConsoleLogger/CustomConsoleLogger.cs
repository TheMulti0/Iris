using System;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class CustomConsoleLogger : ILogger
    {
        private readonly string _name;

        public CustomConsoleLogger(string name) => _name = name;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var formattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            var formattedLevel = logLevel.ToString();
            string formattedMessage = Format(state, exception);

            Console.WriteLine(
                $"[{formattedDate}] [{formattedLevel}] [{_name}] {formattedMessage}");
        }

        private static string Format<TState>(TState state, Exception exception)
        {
            return exception != null 
                ? $"{state}\n{exception}" 
                : state.ToString();
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => new EmptyDisposable();
    }
}