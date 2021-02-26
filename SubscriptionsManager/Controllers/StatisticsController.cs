using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SubscriptionsDataLayer;

namespace SubscriptionsManager
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
        private readonly UserChatSubscriptionEqualityComparer _equalityComparer = new();

        public StatisticsController(
            IChatSubscriptionsRepository chatSubscriptionsRepository)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        [HttpGet]
        public Statistics Get()
        {
            IQueryable<SubscriptionEntity> subscriptions = _chatSubscriptionsRepository
                .Get();
            
            var subscriptionsCount = subscriptions
                .Count();

            var chatsCount = subscriptions
                .SelectMany(entity => entity.Chats)
                .ToList()
                .Distinct(_equalityComparer) // Distinct with custom equality comparer is not supported with mongo IQueryable
                .Count();

            return new Statistics(
                subscriptionsCount,
                chatsCount);
        }
    }
}