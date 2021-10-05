using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PostsListener.Client;
using Scraper.MassTransit.Common;

namespace Dashboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsListenerSubscriptionsController : ControllerBase
    {
        private readonly INewPostSubscriptionsClient _client;

        public PostsListenerSubscriptionsController(INewPostSubscriptionsClient client)
        {
            _client = client;
        }

        [HttpGet]
        public Task<IEnumerable<Subscription>> Get(CancellationToken ct)
        {
            return _client.GetSubscriptionsAsync(ct);
        }

        [HttpPost("{platform}/{id}")]
        public async Task AddOrUpdate(
            string id,
            string platform,
            TimeSpan pollInterval,
            DateTime earliestPostDate,
            CancellationToken ct)
        {
            await _client.AddOrUpdateSubscription(id, platform, pollInterval, earliestPostDate, ct);
        }
        
        [HttpPost("{platform}/{id}/poll")]
        public async Task TriggerPoll(
            string id,
            string platform,
            CancellationToken ct)
        {
            await _client.TriggerPoll(id, platform, ct);
        }
        
        [HttpDelete("{platform}/{id}")]
        public async Task Remove(string id, string platform, CancellationToken ct)
        {
            await _client.RemoveSubscription(id, platform, ct);
        }
    }
}