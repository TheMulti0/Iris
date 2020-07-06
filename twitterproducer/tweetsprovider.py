from twitter_scraper import get_tweets

from producer.updatesprovider import UpdatesProvider
from twitterproducer.tweet import Tweet
from twitterproducer.updatefactory import UpdateFactory


class TweetsProvider(UpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            UpdateFactory.to_update(tweet)
            for tweet in self.get_typed_tweets(user_id)
        ]

    @staticmethod
    def get_typed_tweets(user_id):
        return [
            Tweet(tweet)
            for tweet in get_tweets(user_id, pages=1)
        ]
