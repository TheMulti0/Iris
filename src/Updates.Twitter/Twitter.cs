using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;
using Iris.Api;

namespace Updates.Twitter
{
    public class Twitter : IUpdatesProvider
    {
        private readonly ILogger<Twitter> _logger;
        private readonly int _maxResults;
        private readonly TwitterExecuter _executer;

        public Twitter(
            ILogger<Twitter> logger,
            TwitterConfig config)
        {
            _logger = logger;
            _maxResults = config.MaxResults;
            
            ITwitterCredentials credentials = Auth.SetUserCredentials(
                config.ConsumerKey,
                config.ConsumerSecret,
                config.AccessToken,
                config.AccessTokenSecret);
            
            _executer = new TwitterExecuter(credentials);
            
            _logger.LogInformation("Completed construction");
        }
        
        public async Task<IEnumerable<Update>> GetUpdates(string userName)
        {
            _logger.LogInformation($"GetUpdates requested with author #{userName} (maxResults = {_maxResults})");
            
            IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromScreenName(userName));

            _logger.LogInformation($"Found user #{userName}");

            IEnumerable<ITweet> tweets = await _executer
                .Execute(() => user.GetUserTimelineAsync(_maxResults));
            List<ITweet> tweetsList = tweets.ToList();
            
            _logger.LogInformation($"Found {tweetsList.Count} tweets by {user.ScreenName}");

            return tweetsList
                .Select(UpdateFactory.ToUpdate);
        }
    }
}
