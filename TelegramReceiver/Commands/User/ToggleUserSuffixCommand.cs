﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserSuffixCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public ToggleUserSuffixCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SavedUser savedUser = await SavedUser;
            UserChatSubscription chat = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            chat.ShowSuffix = !chat.ShowSuffix;
            
            await _savedUsersRepository.AddOrUpdateAsync(savedUser.User, chat);

            return new RedirectResult(Route.User);
        }
    }
}