using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Iris.Config;
using Iris.Logging;
using Iris.Watcher;
using Microsoft.Extensions.Logging;

namespace Iris
{
    internal static class TelegramBotApplication
    {
        private static async Task Main(string[] args)
        {
#if DEBUG
            const string configDirectory = "../../../../config";
            const string logsDirectory = "../../../../logs";
#else
            const string configDirectory = "config";
            const string logsDirectory = "logs";
#endif
            
            var config = await JsonSerializer
                .DeserializeAsync<ApplicationConfig>(
                    new FileStream($"{configDirectory}/appsettings.json", FileMode.Open));

            ILoggerFactory factory = LoggerFactory
                .Create(builder => builder
                    .AddConsole()
                    .AddFile(options => options.LogDirectory = logsDirectory));
        
            var telegramBot = new Bot.Bot(
                config,
                factory,
                $"{configDirectory}/{config.TelegramBotConfig.ChatsFile}",
                new JsonUpdateValidator($"{configDirectory}/{config.TelegramBotConfig.SavedUpdatesFile}"));

            await Task.Delay(-1);
        }
    }
}