using System;
using Tweetinvi;
using Tweetinvi.Models;

namespace Updates.Twitter
{
    public class TwitterExecuter
    {
        private readonly ITwitterCredentials _credentials;

        public TwitterExecuter(ITwitterCredentials credentials)
        {
            _credentials = credentials;
        }

        public T Execute<T>(Func<T> operation)
        {
            return Auth
                .ExecuteOperationWithCredentials(_credentials, operation);
        }
    }
}