using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class CustomConsoleDiExtensions
    {
        /// <summary>
        ///     Adds a console logger named 'Console' to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder" /> to use.</param>
        public static ILoggingBuilder AddCustomConsole(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, CustomConsoleLoggerProvider>();
            return builder;
        }
        
        public static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
        {
            builder
                .AddConfiguration(context.Configuration)
                .AddCustomConsole();

            if (context.Configuration.GetSection("Sentry")
                .Exists())
            {
                builder.AddSentry();
            }
        }
    }
}