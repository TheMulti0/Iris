using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramReceiver
{
    public record Context(
        ITelegramBotClient Client,
        IObservable<Update> IncomingUpdates)
    {
        public Update Update { get; init; }

        public void Deconstruct(
            out ITelegramBotClient client,
            out IObservable<Update> incomingUpdates,
            out Update update)
        {
            client = Client;
            incomingUpdates = IncomingUpdates;
            update = Update;
        }
    }
}