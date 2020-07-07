import unittest
from unittest import TestCase

from twitterproducer.tests.mock_tweets_provider import MockTweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider


class TwitterUpdatesProviderTests(TestCase):
    def test1(self):
        user_id = '@mock_user'

        tweets = list(TwitterUpdatesProvider(
            MockTweetsProvider()).get_updates(user_id))
        self.assertNotEqual(0, len(tweets))


if __name__ == '__main__':
    unittest.main()
