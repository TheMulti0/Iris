from tweepy import OAuthHandler, API

from twitterproducer.tweets.itweets_provider import ITweetsProvider


class TweetsProvider(ITweetsProvider):
    def __init__(self, config, logger):
        auth = OAuthHandler(config['consumer_key'], config['consumer_secret'])
        auth.set_access_token(config['access_token'], config['access_token_secret'])

        self.__api = API(auth)

        self.__logger = logger

    def get_tweets(self, user_id):
        def is_tweet_relevant(t):
            if t.in_reply_to_status_id_str is not None:
                if t.in_reply_to_screen_name == user_id:
                    return True
                return False

            return True

        return filter(
            is_tweet_relevant,
            self.__api.user_timeline(screen_name=user_id, tweet_mode="extended"))
