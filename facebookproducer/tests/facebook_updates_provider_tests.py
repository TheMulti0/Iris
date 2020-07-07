import unittest
from unittest import TestCase

from facebookproducer.tests.mock_posts_provider import MockPostsProvider
from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider


class FacebookUpdatesProviderTests(TestCase):
    def test1(self):
        user_id = 'mock_user'

        tweets = list(FacebookUpdatesProvider(
            MockPostsProvider()).get_updates(user_id))
        self.assertNotEqual(0, len(tweets))


if __name__ == '__main__':
    unittest.main()
