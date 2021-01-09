using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class AddUserCommandd : BaseCommandd, ICommandd
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;
        private readonly TimeSpan _defaultInterval;

        public AddUserCommandd(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer,
            TelegramConfig config) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
            _defaultInterval = config.DefaultInterval;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = Trigger.CallbackQuery;

            Platform platform = GetPlatform(query);

            await SendRequestMessage(platform, token);

            // Wait for the user to reply with desired answer
            Update nextUpdate = await NextUpdate;

            if (nextUpdate.Type != UpdateType.Message)
            {
                return new EmptyResult();
            }
            
            Message message = nextUpdate.Message;
            
            var user = new User(message.Text, platform);
            await AddUser(message, user, token);

            return new RedirectResult(
                Route.User,
                Context with { Trigger = null, SelectedUser = user });
        }

        private static Platform GetPlatform(CallbackQuery query)
        {
            return Enum.Parse<Platform>(query.Data.Split("-").Last());
        }

        private Task SendRequestMessage(
            Platform platform,
            CancellationToken token)
        {
            return Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: Trigger.GetMessageId(),
                text: $"{Dictionary.EnterUserFromPlatform} {Dictionary.GetPlatform(platform)}",
                cancellationToken: token);
        }
        
        private async Task AddUser(
            Message message,
            User user,
            CancellationToken token)
        {   
            TimeSpan interval = _defaultInterval;
            
            var userPollRule = new UserPollRule(user, interval);

            _producer.Send(
                new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    ConnectedChat));

            await _savedUsersRepository.AddOrUpdateAsync(
                user,
                new UserChatInfo
                {
                    ChatId = ConnectedChat,
                    Interval = interval,
                    DisplayName = user.UserId,
                    Language = Language
                });

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.Added} {user.UserId}",
                replyToMessageId: message.MessageId,
                cancellationToken: token);
        }
    }
}