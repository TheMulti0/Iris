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
            var config = await JsonSerializer
                .DeserializeAsync<ApplicationConfig>(
                    new FileStream("../../../appsettings.json", FileMode.Open));

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            
            var telegramBot = new TelegramBot(
                config,
                factory.CreateLogger<TelegramBot>(),
                new JsonUpdateValidator("../../../savedUpdates.json"));

            await Task.Delay(-1);
        }
    }
}