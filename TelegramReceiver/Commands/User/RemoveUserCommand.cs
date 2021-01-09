using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;

        public RemoveUserCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer): base(context)
        {
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = Trigger.CallbackQuery;
            
            User user = GetUserBasicInfo(query);

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(user);

            await Remove(user, query.Message, inlineKeyboardMarkup, token);

            return new RedirectResult(Route.Users);
        }

        private async Task Remove(
            User user,
            Message message,
            InlineKeyboardMarkup markup,
            CancellationToken token)
        {
            var userPollRule = new UserPollRule(user, null);
            
            _producer.Send(
                new ChatPollRequest(
                    Request.StopPoll,
                    userPollRule,
                    ConnectedChat));
            
            await _savedUsersRepository.RemoveAsync(user, ConnectedChat);

            await Client.EditMessageTextAsync(
                messageId: message.MessageId,
                chatId: ContextChat,
                text: $"{Dictionary.Removed} ({user.UserId})",
                replyMarkup: markup,
                cancellationToken: token);
        }

        private InlineKeyboardMarkup CreateMarkup(User user)
        {
            (string userId, Platform platform) = user;
            
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.User}-{userId}-{Enum.GetName(platform)}"));
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(
                items[^2],
                Enum.Parse<Platform>(items[^1]));
        }
    }
}