using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using UserDataLayer;

namespace SubscriptionsManager
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public SubscriptionsController(
            ISavedUsersRepository savedUsersRepository)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        [HttpGet]
        public ValueTask<List<Subscription>> Get()
        {
            return _savedUsersRepository.GetAll()
                .ToAsyncEnumerable()
                .Select(ToSubscription)
                .ToListAsync();
        }

        private static Subscription ToSubscription(SavedUser user)
        {
            IOrderedEnumerable<UserChatSubscription> orderedByInterval = user.Chats.OrderBy(info => info.Interval);

            return new Subscription(
                user.User,
                orderedByInterval.FirstOrDefault()?.Interval);
        }
    }
}