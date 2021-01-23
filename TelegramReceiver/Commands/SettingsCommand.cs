using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramReceiver
{
    internal class SettingsCommand : BaseCommand, ICommand
    {
        public SettingsCommand(Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Chat connectedChatInfo = Context.ConnectedChat ?? await Client.GetChatAsync(ConnectedChat, token);
            
            string text = $"{Dictionary.SettingsFor} {GetChatTitle(connectedChatInfo)}";
            var markup = GetMarkup();

            if (Trigger.Type == UpdateType.CallbackQuery)
            {
                await Client.EditMessageTextAsync(
                    chatId: ContextChat,
                    messageId: Trigger.CallbackQuery.Message.MessageId,
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
            }
            else
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
            }

            return new NoRedirectResult();
        }

        private InlineKeyboardMarkup GetMarkup()
        {
            InlineKeyboardButton[] buttons = {
                InlineKeyboardButton.WithCallbackData(
                    $"{Dictionary.Subscriptions}",
                    Route.Platforms.ToString()),

                InlineKeyboardButton.WithCallbackData(
                    $"{Dictionary.Language}",
                    Route.Language.ToString())
            };

            return new InlineKeyboardMarkup(buttons.Batch(1));
        }
    }
}