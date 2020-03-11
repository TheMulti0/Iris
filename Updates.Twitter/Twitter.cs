using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Updates.Api;

namespace Updates.Twitter
{
    public class Twitter : IUpdatesProvider
    {
        private readonly int _maxResults;
        private readonly TwitterExecuter _executer;

        public Twitter(
            int maxResults,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret)
        {
            _maxResults = maxResults;
            ITwitterCredentials credentials = Auth.SetUserCredentials(
                consumerKey,
                consumerSecret,
                accessToken,
                accessTokenSecret);
            
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
