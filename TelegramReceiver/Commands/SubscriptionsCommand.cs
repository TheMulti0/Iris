using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using SubscriptionsDb;

namespace TelegramReceiver
{
    internal class SubscriptionsCommand : BaseCommand, ICommand
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public SubscriptionsCommand(
            Context context,
            IChatSubscriptionsRepository chatSubscriptionsRepository) : base(context)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            List<SubscriptionEntity> currentUsers = _chatSubscriptionsRepository
                .Get()
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

        private (InlineKeyboardMarkup, string) GetMessageDetails(IReadOnlyCollection<SubscriptionEntity> currentUsers)
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

        private InlineKeyboardMarkup GetUsersMarkup(IReadOnlyCollection<SubscriptionEntity> users)
        {
            IEnumerable<InlineKeyboardButton> userButtons = users
                .Select(UserToButton)
                .ToList();

            bool canAddUsers = users.Count <= 4 || IsSuperUser;
            if (canAddUsers)
            {
                userButtons = userButtons.Concat(GetAddUserButton());
            }
            
            return new InlineKeyboardMarkup(userButtons.Concat(GetBackButton()).Batch(1));
        }

        private InlineKeyboardButton UserToButton(SubscriptionEntity user)
        {
            return InlineKeyboardButton.WithCallbackData(
                $"{user.User.UserId}",
                $"{Route.User.ToString()}-{user.Id}");
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
    }
}