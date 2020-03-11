using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Logic.Model;
using Tweetinvi.Models;
using Updates.Api;
using Updates.Configs;

namespace Updates.Twitter
{
    public class Twitter : IUpdatesProvider
    {
        private readonly int _maxResults;
        private readonly TwitterExecuter _executer;

        public Twitter(TwitterConfig config)
        {
            _maxResults = config.MaxResults;
            ITwitterCredentials credentials = Auth.SetUserCredentials(
                config.ConsumerKey,
                config.ConsumerSecret,
                config.AccessToken,
                config.AccessTokenSecret);
            
            _executer = new TwitterExecuter(credentials);
        }
        
        public async Task<IEnumerable<IUpdate>> GetUpdates(long authorId)
        {
            Tweetinvi.Models.IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromId(authorId));

            IEnumerable<ITweet> tweets = await _executer
                .Execute(() => user.GetUserTimelineAsync(_maxResults));

            return tweets.Select(itweet => new Tweet(itweet));
        }
    }
}
