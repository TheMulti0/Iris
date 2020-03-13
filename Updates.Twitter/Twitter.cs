using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Logic.Model;
using Tweetinvi.Models;
using Updates.Api;
using Updates.Configs;

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
        
        public async Task<IEnumerable<Update>> GetUpdates(long authorId)
        {
            _logger.LogInformation($"GetUpdates requested with author #{authorId} (maxResults = {_maxResults})");
            
            Tweetinvi.Models.IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromId(authorId));
            
            _logger.LogInformation($"Found user #{authorId}");

            IEnumerable<ITweet> tweets = await _executer
                .Execute(() => user.GetUserTimelineAsync(_maxResults));
            List<ITweet> tweetsList = tweets.ToList();
            
            _logger.LogInformation($"Found {tweetsList.Count} tweets by #{authorId}");

            return tweetsList
                .Select(UpdateFactory.ToUpdate);
        }
    }
}
