using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ScrapersDistributor
{
    internal class PollRulesManagerClient : IPollRulesManagerClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public PollRulesManagerClient(PollRulesPollerConfig config)
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

        public async Task<List<UserPollRule>> Get(CancellationToken token)
        {
            string response = await _client.GetStringAsync("pollRules", token);

            return JsonSerializer.Deserialize<List<UserPollRule>>(response, _jsonSerializerOptions);
        }
    }
}