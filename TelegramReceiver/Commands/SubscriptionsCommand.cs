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
                    user => user.User.Platform == SelectedPlatform &&
                            user.Chats.Any(chat => chat.ChatId == ConnectedChat))
                .ToList();

            (InlineKeyboardMarkup markup, string text) = GetMessageDetails(currentUsers);

            if (Trigger?.Type == UpdateType.CallbackQuery)
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
            var platform = Dictionary.GetPlatform(SelectedPlatform ?? throw new NullReferenceException());
            
            return currentUsers.Any() 
                ? (GetUsersMarkup(currentUsers), $"{currentUsers.Count} {Dictionary.UsersFound} ({platform})") 
                : (GetNoUsersMarkup(), $"{Dictionary.NoUsersFound} ({platform})");
        }

        private InlineKeyboardMarkup GetNoUsersMarkup()
        {
            return new(
                GetAddUserButton()
                    .Concat(GetBackButton())
                    .Batch(1));
        }

        private InlineKeyboardMarkup GetUsersMarkup(IReadOnlyCollection<SavedUser> users)
        {
            IEnumerable<InlineKeyboardButton> userButtons = users
                .Select(UserToButton)
                .ToList();
            
            if (users.Count <= 4 && !IsSuperUser)
            {
                userButtons = userButtons.Concat(GetAddUserButton());
            }
            
            return new InlineKeyboardMarkup(userButtons.Concat(GetBackButton()).Batch(1));
        }

        private InlineKeyboardButton[] GetAddUserButton() => new[]
        {
            InlineKeyboardButton.WithCallbackData(
                Dictionary.AddUser,
                $"{Route.AddUser}-{SelectedPlatform}")
        };

        private InlineKeyboardButton[] GetBackButton() => new[]
        {
            InlineKeyboardButton.WithCallbackData(
                Dictionary.Back,
                Route.Platforms.ToString())
        };

        private InlineKeyboardButton UserToButton(SavedUser user)
        {
            (string userId, Platform platform) = user.User;

            UserChatSubscription chatSubscription = user.Chats.First(subscription => subscription.ChatId == ConnectedChat);
            
            return InlineKeyboardButton.WithCallbackData(
                $"{chatSubscription.DisplayName}",
                $"{Route.User.ToString()}-{userId}-{Enum.GetName(platform)}");
        }
    }
}