using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ProducerApi;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterProducer
{
    public class Twitter : IProducer
    {
        private readonly TwitterExecuter _executer;
        private readonly Subject<IUpdate> _updates;

        public IObservable<IUpdate> Updates => _updates;

        public Twitter(
            IEnumerable<long> watchedUsersIds,
            TimeSpan pollInterval,
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
            
            _updates = new Subject<IUpdate>();

            Observable
                .Interval(pollInterval)
                .Subscribe(async _ =>
                    {
                        foreach (long id in watchedUsersIds)
                        {
                            await foreach (IUpdate update in ToUpdatesAsync(GetPostsByUserTimeline(id)))
                            {
                                _updates.OnNext(update);
                            }
                        }
                    });
        }

        private async Task<IEnumerable<ITweet>> GetPostsByUserTimeline(long authorId)
        {
            Tweetinvi.Models.IUser user = await _executer
                .Execute(() => UserAsync.GetUserFromId(authorId));

            return await _executer
                .Execute(() => user.GetUserTimelineAsync());
        }

        private static async IAsyncEnumerable<IUpdate> ToUpdatesAsync(Task<IEnumerable<ITweet>> tweets)
        {
            IAsyncEnumerable<ITweet> result = await tweets.FlattenAsync();

            await foreach (ITweet tweet in result)
            {
                yield return new Tweet(tweet);
            }
        }
    }
}
