using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace UserDataLayer
{
    public class MockSavedUsersRepository : ISavedUsersRepository
    {
        public IQueryable<SavedUser> Get()
        {
            return new EnumerableQuery<SavedUser>(
                new SavedUser[]
                {
                    new()
                    {
                        User = new User("user", "user", "mock"),
                        Chats = new List<ChatInfo>()
                    }
                });
        }
        
        public Task<SavedUser> GetAsync(User user)
        {
            return Task.FromResult(
                new SavedUser
                {
                    User = user,
                    Chats = new List<ChatInfo>()
                });
        }

        public Task AddOrUpdateAsync(User user, ChatInfo chat)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync(User user, ChatInfo chat)
        {
            return Task.CompletedTask;
        }
    }
}