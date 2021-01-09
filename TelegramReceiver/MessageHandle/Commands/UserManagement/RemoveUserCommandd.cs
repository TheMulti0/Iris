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
    internal class RemoveUserCommandd : ICommandd
    {
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly LanguageDictionary _dictionary;
        
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;

        public RemoveUserCommandd(
            Context context,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer)
        {
            (_client, _, _update, _contextChat, _connectedChat, _, _dictionary) = context;
            
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = _update.CallbackQuery;
            
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
                    _connectedChat));
            
            await _savedUsersRepository.RemoveAsync(user, _connectedChat);

            await _client.EditMessageTextAsync(
                messageId: message.MessageId,
                chatId: _contextChat,
                text: $"{_dictionary.Removed} ({user.UserId})",
                replyMarkup: markup,
                cancellationToken: token);
        }

        private InlineKeyboardMarkup CreateMarkup(User user)
        {
            (string userId, Platform platform) = user;
            
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    _dictionary.Back,
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