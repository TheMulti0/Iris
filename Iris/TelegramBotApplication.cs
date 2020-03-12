using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Iris.Config;
using Microsoft.Extensions.Logging;
using Updates.Watcher;

namespace Iris
{
    internal static class TelegramBotApplication
    {
        private static async Task Main(string[] args)
        {
#if DEBUG
            const string rootDirectory = "../../..";
#else
            const string rootDirectory = "data";
#endif
            
            var config = await JsonSerializer
                .DeserializeAsync<ApplicationConfig>(
                    new FileStream($"{rootDirectory}/appsettings.json", FileMode.Open));

            ILoggerFactory factory = LoggerFactory
                .Create(builder => builder
                    .AddConsole()
                    .AddFile(options => options.LogDirectory = $"{rootDirectory}/logs"));
        
            var telegramBot = new TelegramBot(
                config,
                factory.CreateLogger<TelegramBot>(),
                new JsonUpdateValidator($"{rootDirectory}/savedUpdates.json"));

            await Task.Delay(-1);
        }
    }
}