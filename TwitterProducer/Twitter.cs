using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProducerApi;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterProducer
{
    public class Twitter : IProducer
    {
        private readonly TwitterExecuter _executer;

        public Twitter(
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret)
        {
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
                .Execute(() => user.GetUserTimelineAsync());

            return tweets.Select(itweet => new Tweet(itweet));
        }
    }
}
