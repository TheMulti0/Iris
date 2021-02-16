using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class SetUserDisplayNameCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public SetUserDisplayNameCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
        }
        
        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            CallbackQuery query = Trigger.CallbackQuery;

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(await SavedUser);

            await SendRequestMessage(query.Message, inlineKeyboardMarkup, token);

            // Wait for the user to reply with desired display name

            var update = await GetNextMessage();

            if (update == null)
            {
                return new NoRedirectResult();
            }

            await SetDisplayName(await SavedUser, update);

            return new RedirectResult(Route.User, Context with { Trigger = null });
        }

        private InlineKeyboardMarkup CreateMarkup(SavedUser user)
        {
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    Dictionary.Back,
                    $"{Route.User}-{user.Id}"));
        }

        private Task SendRequestMessage(
            Message message,
            InlineKeyboardMarkup markup,
            CancellationToken token)
        {
            return Client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: Dictionary.EnterNewDisplayName,
                replyMarkup: markup,
                cancellationToken: token);
        }

        private async Task SetDisplayName(
            SavedUser savedUser,
            Update update)
        {
            UserChatSubscription chat = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            string newDisplayName = update.Message.Text;
            chat.DisplayName = newDisplayName;
            
            await _savedUsersRepository.AddOrUpdateAsync(savedUser.User, chat);
        }
    }
}