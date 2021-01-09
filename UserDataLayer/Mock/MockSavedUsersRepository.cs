using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace UserDataLayer
{
    public class MockSavedUsersRepository : ISavedUsersRepository
    {
        public IQueryable<SavedUser> GetAll()
        {
            return new EnumerableQuery<SavedUser>(
                new SavedUser[]
                {
                    new()
                    {
                        User = new User("user", Platform.Facebook),
                        Chats = new List<UserChatSubscription>()
                    }
                });
        }
        
        public Task<SavedUser> GetAsync(User user)
        {
            return Task.FromResult(
                new SavedUser
                {
                    User = user,
                    Chats = new List<UserChatSubscription>()
                });
        }

        public Task AddOrUpdateAsync(User user, UserChatSubscription chat)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync(User user, string chatId)
        {
            return Task.CompletedTask;
        }
    }
}