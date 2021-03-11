﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using SubscriptionsDb;

namespace SubscriptionsManager
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;

        public SubscriptionsController(
            IChatSubscriptionsRepository chatSubscriptionsRepository)
        {
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
        }

        [HttpGet]
        public IEnumerable<Subscription> Get()
        {
            return _chatSubscriptionsRepository
                .Get()
                .AsEnumerable()
                .Select(ToSubscription);
        }

        private static Subscription ToSubscription(SubscriptionEntity user)
        {
            IOrderedEnumerable<UserChatSubscription> orderedByInterval = user.Chats.OrderBy(info => info.Interval);

            return new Subscription(
                user.User,
                orderedByInterval.FirstOrDefault()?.Interval);
        }
    }
}