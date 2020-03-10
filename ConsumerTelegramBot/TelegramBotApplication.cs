using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ConsumerTelegramBot.Configuration;

namespace ConsumerTelegramBot
{
    internal class TelegramBotApplication
    {
        private static async Task Main(string[] args)
        {
            var config = await JsonSerializer
                .DeserializeAsync<ConsumerTelegramBotConfig>(
                    new FileStream("../../../appsettings.json", FileMode.Open));

            var telegramBot = new TelegramBot(config);

            await Task.Delay(-1);
        }
    }
}