from twitter_scraper import get_tweets

from producer.iupdates_provider import IUpdatesProvider
from twitterproducer.tweet import Tweet
from twitterproducer.updatefactory import UpdateFactory


class TweetsProvider(IUpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            UpdateFactory.to_update(tweet)
            for tweet in self.__get_tweets(user_id)
        ]

    @staticmethod
    def __get_tweets(user_id):
        return [
            Tweet(tweet)
            for tweet in get_tweets(user_id, pages=1)
        ]
