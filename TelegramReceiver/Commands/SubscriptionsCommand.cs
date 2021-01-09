using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class SubscriptionsCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public SubscriptionsCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            List<SavedUser> currentUsers = _savedUsersRepository
                .GetAll()
                .Where(
                    user => user.Chats
                        .Any(chat => chat.ChatId == ConnectedChat))
                .ToList();

            (InlineKeyboardMarkup markup, string text) = GetMessageDetails(currentUsers);

            if (Trigger.Type == UpdateType.CallbackQuery)
            {
                await Client.EditMessageTextAsync(
                    chatId: ContextChat,
                    messageId: Trigger.GetMessageId(),
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

        private (InlineKeyboardMarkup, string) GetMessageDetails(IReadOnlyCollection<SavedUser> currentUsers)
        {
            return currentUsers.Any() 
                ? (GetUsersMarkup(currentUsers), $"{currentUsers.Count} {Dictionary.UsersFound}") 
                : (GetNoUsersMarkup(), Dictionary.NoUsersFound);
        }

        private InlineKeyboardButton GetAddUserButton()
        {
            return InlineKeyboardButton.WithCallbackData(
                Dictionary.AddUser,
                Route.SelectPlatform.ToString());
        }

        private InlineKeyboardMarkup GetNoUsersMarkup()
        {
            var buttons = new[]
            {
                new[]
                {
                    GetAddUserButton()
                }
            };

            return new InlineKeyboardMarkup(buttons);
        }

        private InlineKeyboardMarkup GetUsersMarkup(IEnumerable<SavedUser> users)
        {
            IEnumerable<IEnumerable<InlineKeyboardButton>> userButtons = users
                .Select(UserToButton)
                .Batch(2)
                .Concat(
                    new[]
                    {
                        new[]
                        {
                            GetAddUserButton()
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(
                                Dictionary.Back,
                                Route.Settings.ToString()), 
                        }
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }

        private static InlineKeyboardButton UserToButton(SavedUser user)
        {
            (string userId, Platform platform) = user.User;

            return InlineKeyboardButton.WithCallbackData(
                $"{user.User}",
                $"{Route.User.ToString()}-{userId}-{Enum.GetName(platform)}");
        }
    }
}