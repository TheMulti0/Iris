using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramReceiver
{
    internal class SelectPlatformCommand : ICommand
    {
        private readonly ITelegramBotClient _client;
        private readonly Telegram.Bot.Types.Update _update;
        private readonly ChatId _contextChat;
        private readonly LanguageDictionary _dictionary;

        public SelectPlatformCommand(
            Context context)
        {
            (_client, _, _update, _contextChat, _, _, _dictionary) = context;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await _client.EditMessageTextAsync(
                chatId: _contextChat,
                messageId: _update.GetMessageId(),
                text: _dictionary.SelectPlatform,
                replyMarkup: GetMarkup(),
                cancellationToken: token);

            return new EmptyResult();
        }
        
        private InlineKeyboardMarkup GetMarkup()
        {
            InlineKeyboardButton ToButton(Platform platform)
            {
                return InlineKeyboardButton.WithCallbackData(
                    _dictionary.GetPlatform(platform),
                    $"{Route.AddUser.ToString()}-{Enum.GetName(platform)}");
            }
            
            IEnumerable<IEnumerable<InlineKeyboardButton>> userButtons = Enum.GetValues<Platform>()
                .Select(ToButton)
                .Batch(2)
                .Concat(
                    new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                _dictionary.Back,
                                Route.Users.ToString())                            
                        }
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }
    }
}