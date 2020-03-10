using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ConsumerTelegramBot.Configuration;
using ProducerApi;
using Telegram.Bot;
using TwitterProducer;

namespace ConsumerTelegramBot
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = await JsonSerializer
                .DeserializeAsync<MediaForwarderConfig>(
                    new FileStream("../../../appsettings.json", FileMode.Open));
            
            var client = new TelegramBotClient(config.Token);

            TwitterConfig twitterConfig = config.TwitterConfig;
            
            IProducer producer = new Twitter(
                twitterConfig.WatchedUsersIds,
                TimeSpan.FromSeconds(twitterConfig.PollIntervalSeconds),
                twitterConfig.ConsumerKey,
                twitterConfig.ConsumerSecret,
                twitterConfig.AccessToken,
                twitterConfig.AccessTokenSecret);

            producer.Updates.Subscribe(async update =>
            {
                foreach (long channelId in config.PostChannelIds)
                {
                    await client.SendTextMessageAsync(channelId, update.Url);                    
                }
            });
            client.StartReceiving();
            await Task.Delay(-1);
        }
    }
}
