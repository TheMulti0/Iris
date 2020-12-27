using System;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace UserDataLayer
{
    public interface ISavedUsersRepository
    {
        IQueryable<SavedUser> GetAll();
        
        Task<SavedUser> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, ChatInfo chat);

        Task RemoveAsync(User user, string chatId);
    }
}