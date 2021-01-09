using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ScrapersDistributor
{
    internal class SubscriptionsManagerClient : ISubscriptionsManagerClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public SubscriptionsManagerClient(SubscriptionsPollerConfig config)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(config.ManagerUrl)
            };
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new TimeSpanConverter(), new NullableTimeSpanConverter()
                }
            };
        }

        public async Task<List<Subscription>> Get(CancellationToken token)
        {
            string response = await _client.GetStringAsync("subscriptions", token);

            return JsonSerializer.Deserialize<List<Subscription>>(response, _jsonSerializerOptions);
        }
    }
}