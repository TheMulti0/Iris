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
    internal class AddUserCommandd : ICommandd
    {
        private readonly Context _context;
        
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly Language _language;
        private readonly LanguageDictionary _dictionary;
        private readonly Task<Update> _nextMessageTask;

        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;
        private readonly TimeSpan _defaultInterval;

        public AddUserCommandd(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer,
            TelegramConfig config)
        {
            _context = context;
            
            (_client, _, _update, _contextChat, _connectedChat, _language, _dictionary) = context;
            _nextMessageTask = context.NextMessageTask;
            
            _dictionary = context.LanguageDictionary;
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
            _defaultInterval = config.DefaultInterval;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = _update.CallbackQuery;

            Platform platform = GetPlatform(query);

            await SendRequestMessage(platform);

            // Wait for the user to reply with desired answer
            Update nextMessage = await _nextMessageTask;
            Message message = nextMessage.Message;
            
            var user = new User(message.Text, platform);
            await AddUser(message, user);

            return new RedirectResult(
                Route.User,
                _context with { Trigger = null, SelectedSavedUser = user });
        }

        private static Platform GetPlatform(CallbackQuery query)
        {
            return Enum.Parse<Platform>(query.Data.Split("-").Last());
        }

        private Task SendRequestMessage(
            Platform platform)
        {
            return _client.EditMessageTextAsync(
                chatId: _contextChat,
                messageId: _update.GetMessageId(),
                text: $"{_dictionary.EnterUserFromPlatform} {_dictionary.GetPlatform(platform)}");
        }
        
        private async Task AddUser(
            Message message,
            User user)
        {   
            TimeSpan interval = _defaultInterval;
            
            var userPollRule = new UserPollRule(user, interval);

            _producer.Send(
                new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    _connectedChat));

            await _savedUsersRepository.AddOrUpdateAsync(
                user,
                new UserChatInfo
                {
                    ChatId = _connectedChat,
                    Interval = interval,
                    DisplayName = user.UserId,
                    Language = _language
                });

            await _client.SendTextMessageAsync(
                chatId: _contextChat,
                text: $"{_dictionary.Added} {user.UserId}",
                replyToMessageId: message.MessageId);
        }
    }
}