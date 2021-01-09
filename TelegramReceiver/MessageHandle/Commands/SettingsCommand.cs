using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class SettingsCommand : BaseCommandd, ICommand
    {
        public SettingsCommand(Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var chat = await Client.GetChatAsync(ConnectedChat, token);

            string text = $"{Dictionary.SettingsFor} {chat.Title}";
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

            return new EmptyResult();
        }

        private InlineKeyboardMarkup GetMarkup()
        {
            InlineKeyboardButton[] buttons = {
                InlineKeyboardButton.WithCallbackData(
                    $"{Dictionary.UsersFound}",
                    Route.Users.ToString()),

                InlineKeyboardButton.WithCallbackData(
                    $"{Dictionary.Language}",
                    Route.Language.ToString())
            };

            return new InlineKeyboardMarkup(buttons.Batch(3));
        }
    }
}