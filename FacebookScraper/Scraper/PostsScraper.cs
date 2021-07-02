using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FacebookScraper
{
    internal class PostsScraper
    {
        private const string FacebookScriptName = "get_posts.py";

        private readonly FacebookUpdatesProviderConfig _config;

        private readonly SemaphoreSlim _proxyIndexLock = new(1, 1);
        private int _proxyIndex;

        public PostsScraper(
            FacebookUpdatesProviderConfig config)
        {
            _config = config;
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(User user)
        {
            var response = await GetResponseAsync(user);

            switch (response.Error)
            {
                case "ProxyError":
                    throw new InvalidOperationException($"proxy is invalid {response.OriginalRequest.Proxy}");
            }
            
            return response.Posts.Select(raw => raw.ToPost());
        }

        private async Task<GetPostsResponse> GetResponseAsync(User user)
        {
            var request = new GetPostsRequest
            {
                UserId = user.UserId,
                Pages = _config.PageCount,
                Proxy = await GetProxyAsync()
            };

            string json = JsonConvert.SerializeObject(request)
                .Replace("\"", "\\\""); // Python argument's double quoted strings need to be escaped

            string responseStr = await ScriptExecutor.ExecutePython(
                FacebookScriptName,
                token: default,
                json);

            var response = JsonConvert.DeserializeObject<GetPostsResponse>(responseStr);
            
            return response with { OriginalRequest = request };
        }

        private async Task<string> GetProxyAsync()
        {
            if (_config.Proxies.Length == 0)
            {
                return null;
            }
            
            await _proxyIndexLock.WaitAsync();

            try
            {
                if (_proxyIndex == _config.Proxies.Length - 1)
                {
                    _proxyIndex = 0;
                }
                else
                {
                    _proxyIndex++;
                }
                
                return _config.Proxies[_proxyIndex];
            }
            finally
            {
                _proxyIndexLock.Release();
            }
        }
    }
}