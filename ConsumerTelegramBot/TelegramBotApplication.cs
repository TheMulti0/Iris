using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ConsumerTelegramBot.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsumerTelegramBot
{
    internal class TelegramBotApplication
    {
        private static async Task Main(string[] args)
        {
            var config = await JsonSerializer
                .DeserializeAsync<ConsumerTelegramBotConfig>(
                    new FileStream("../../../appsettings.json", FileMode.Open));

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            
            var telegramBot = new TelegramBot(config, factory.CreateLogger<TelegramBot>());

            await Task.Delay(-1);
        }
    }
}