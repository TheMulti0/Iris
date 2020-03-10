using System;
using System.Collections.Generic;
using ConsumerTelegramBot.Configuration;
using ProducerApi;
using Telegram.Bot;
using TwitterProducer;

namespace ConsumerTelegramBot
{
    internal class TelegramBot
    {
        private readonly ConsumerTelegramBotConfig _config;
        private readonly ITelegramBotClient _client;

        public TelegramBot(ConsumerTelegramBotConfig config)
        {
            _config = config;
            
            _client = new TelegramBotClient(config.Token);
            _client.StartReceiving();

            RegisterProducers();
        }

        private void RegisterProducers()
        {
            foreach (IProducer producer in GetProducers())
            {
                producer.Updates.Subscribe(OnProducerUpdate);
            }
        }

        private async void OnProducerUpdate(IUpdate update)
        {
            foreach (long channelId in _config.PostChannelIds)
            {
                await _client.SendTextMessageAsync(channelId, update.Url);
            }
        }

        private IEnumerable<IProducer> GetProducers()
        {
            TwitterConfig twitterConfig = _config.TwitterConfig;

            return new[]
            {
                new Twitter(
                    twitterConfig.WatchedUsersIds,
                    TimeSpan.FromSeconds(twitterConfig.PollIntervalSeconds),
                    twitterConfig.ConsumerKey,
                    twitterConfig.ConsumerSecret,
                    twitterConfig.AccessToken,
                    twitterConfig.AccessTokenSecret)
            };
        }
    }
}