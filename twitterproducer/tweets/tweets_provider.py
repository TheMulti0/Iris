from tweepy import OAuthHandler, API

from twitterproducer.tweets.itweets_provider import ITweetsProvider


class TweetsProvider(ITweetsProvider):
    def __init__(self, config, logger):
        auth = OAuthHandler(config['consumer_key'], config['consumer_secret'])
        auth.set_access_token(config['access_token'], config['access_token_secret'])

        self.__api = API(auth)

        self.__logger = logger

    def get_tweets(self, user_id):
        return self.__api.user_timeline(screen_name=user_id, tweet_mode="extended")
