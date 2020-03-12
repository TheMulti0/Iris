using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetEscapades.Extensions.Logging.RollingFile.Internal;

namespace Iris
{
    [ProviderAlias("Console")]
    public class ConsoleLoggerProvider : BatchingLoggerProvider
    {
        public ConsoleLoggerProvider(IOptionsMonitor<BatchingLoggerOptions> options) : base(options)
        {
        }

        protected override Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token)
        {
            // ReSharper disable once SuggestVarOrType_Elsewhere
            foreach (var group in messages.GroupBy(GetGrouping))
            {
                foreach (LogMessage item in group)
                {
                    Console.WriteLine(item.Message);
                }
            }
            
            return Task.CompletedTask;
        }

        private static (int Year, int Month, int Day, int Hour, int Minute) GetGrouping(LogMessage message)
        {
            return (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day, message.Timestamp.Hour, message.Timestamp.Minute);
        }
    }
}