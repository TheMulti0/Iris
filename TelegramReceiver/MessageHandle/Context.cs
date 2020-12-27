using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramReceiver
{
    public record Context(
        ITelegramBotClient Client,
        IObservable<Update> IncomingUpdates,
        Update Update);
}