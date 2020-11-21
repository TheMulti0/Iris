import logging

from tweepy import OAuthHandler, API

from twitterproducer.tweets.itweets_provider import ITweetsProvider


class TweetsProvider(ITweetsProvider):
    def __init__(self, config):
        auth = OAuthHandler(config['consumer_key'], config['consumer_secret'])
        auth.set_access_token(config['access_token'], config['access_token_secret'])

        self.__api = API(auth)

        self.__logger = logging.getLogger(TweetsProvider.__name__)

    def get_tweets(self, user_id):
        def is_tweet_relevant(t):
            if t.in_reply_to_status_id_str is None:
                # Tweet is not a reply, filter in
                return True
            else:
                if t.in_reply_to_screen_name == user_id:
                    # Tweet is a reply to a tweet by this user, filter in
                    return True
                else:
                    # Tweet is a reply to a tweet by another user, filter out
                    return False

        return filter(
            is_tweet_relevant,
            self.__api.user_timeline(screen_name=user_id, tweet_mode="extended"))
