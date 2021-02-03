using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace UserDataLayer
{
    public interface ISavedUsersRepository
    {
        IQueryable<SavedUser> GetAll();
        
        Task<SavedUser> GetAsync(ObjectId id);
        
        Task<SavedUser> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, UserChatSubscription chat);

        Task RemoveAsync(User user, string chatId);
    }
}