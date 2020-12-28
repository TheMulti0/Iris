using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class SelectPlatformCommand : ICommand
    {
        public const string CallbackPath = "selectPlatform";
        public ITrigger[] Triggers { get; } = {
            new CallbackTrigger(CallbackPath)
        };

        public Task OperateAsync(Context context )
        {
            Message message = context.Update.CallbackQuery.Message;
            
            return context.Client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: context.LanguageDictionary.SelectPlatform,
                replyMarkup: GetMarkup(context));
        }
        
        private InlineKeyboardMarkup GetMarkup(Context context)
        {
            InlineKeyboardButton ToButton(Platform platform)
            {
                return InlineKeyboardButton.WithCallbackData(
                    context.LanguageDictionary.GetPlatform(platform),
                    $"{AddUserCommand.CallbackPath}-{Enum.GetName(platform)}");
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
                                context.LanguageDictionary.Back,
                                UsersCommand.CallbackPath)                            
                        }
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }
    }
}