using System;
using System.Threading.Tasks;

namespace Iris
{
    internal static class TelegramBotApplication
    {
        private static async Task Main(string[] args)
        {
            
            Console.Write("HELLO!!!!!!!! \n \n \n ");
            Console.ReadLine();
            await Task.Delay(-1);
// #if DEBUG
//             const string rootDirectory = "../../..";
// #else
//             const string rootDirectory = "config";
// #endif
//             
//             var config = await JsonSerializer
//                 .DeserializeAsync<ApplicationConfig>(
//                     new FileStream($"{rootDirectory}/appsettings.json", FileMode.Open));
//
//             ILoggerFactory factory = LoggerFactory
//                 .Create(builder => builder
//                     .AddConsole()
//                     .AddFile(options => options.LogDirectory = "logs"));
//         
//             var telegramBot = new Bot.Bot(
//                 config,
//                 factory,
//                 $"{rootDirectory}/{config.TelegramBotConfig.ChatsFile}",
//                 new JsonUpdateValidator($"{rootDirectory}/{config.TelegramBotConfig.SavedUpdatesFile}"));
//
//             await Task.Delay(-1);
        }
    }
}