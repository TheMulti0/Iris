using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Extensions
{
    public static class TestsLoggerDiExtensions
    {
        public static ILoggingBuilder AddTestsLogging(
            this ILoggingBuilder builder,
            TestContext context)
        {
            builder.Services.AddSingleton<ILoggerProvider>(
                new TestsLoggerProvider(context));
            return builder;
        }
    }
}