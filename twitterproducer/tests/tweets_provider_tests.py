import json
import logging
import unittest
from unittest import TestCase

from twitterproducer.tweets.tweets_provider import TweetsProvider


class TweetsProviderTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(TweetsProviderTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

    def test1(self):
        user_id = '@realDonaldTrump'

        config = json.load(open('twitterconfig.json', encoding='utf-8'))

        tweets = list(
            TweetsProvider(config).get_tweets(user_id))

        self.assertNotEqual(0, len(tweets))


if __name__ == '__main__':
    unittest.main()
