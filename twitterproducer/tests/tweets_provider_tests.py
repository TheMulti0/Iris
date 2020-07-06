import unittest
from unittest import TestCase

from twitterproducer.tweets_provider import TweetsProvider


class TweetsProviderTests(TestCase):
    def test1(self):
        user_id = '@realDonaldTrump'

        posts = TweetsProvider().get_updates(user_id)
        self.assertFalse(len(posts) == 0)


if __name__ == '__main__':
    unittest.main()
