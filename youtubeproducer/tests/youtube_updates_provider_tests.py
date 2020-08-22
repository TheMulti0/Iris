import unittest
from unittest import TestCase

from youtubeproducer.tests.mock_videos_provider import MockVideosProvider
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider


class YouTubeUpdatesProviderTests(TestCase):
    def test1(self):
        user_id = 'mock_user'

        videos = list(YouTubeUpdatesProvider(
            MockVideosProvider()).get_updates(user_id))
        self.assertNotEqual(0, len(videos))


if __name__ == '__main__':
    unittest.main()
