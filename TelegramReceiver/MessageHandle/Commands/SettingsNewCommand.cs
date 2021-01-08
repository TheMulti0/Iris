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
    internal class SettingsNewCommand : INewCommand
    {
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly LanguageDictionary _dictionary;

        public SettingsNewCommand(
            Context context)
        {
            (_client, _, _update, _contextChat, _connectedChat, _, _dictionary) = context;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var chat = await _client.GetChatAsync(_connectedChat, token);

            string text = $"{_dictionary.SettingsFor} {chat.Title}";
            var markup = GetMarkup();

            if (_update.Type == UpdateType.CallbackQuery)
            {
                await _client.EditMessageTextAsync(
                    chatId: _contextChat,
                    messageId: _update.CallbackQuery.Message.MessageId,
                    text: text,
                    replyMarkup: markup,
                    cancellationToken: token);
                
            }
            else
            {
                await _client.SendTextMessageAsync(
                    chatId: _contextChat,
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
                    $"{_dictionary.UsersFound}",
                    Route.Users.ToString()),

                InlineKeyboardButton.WithCallbackData(
                    $"{_dictionary.Language}",
                    Route.SetLanguage.ToString())
            };

            return new InlineKeyboardMarkup(buttons.Batch(3));
        }
    }
}