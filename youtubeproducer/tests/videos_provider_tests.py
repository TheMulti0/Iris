import logging
import unittest
from unittest import TestCase

from youtubeproducer.videos.videos_provider import VideosProvider


class VideosProviderTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(VideosProviderTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

    def test1(self):
        channel_id = 'UCvjXo25nY-WMCTEXZZb0xsw'

        config = {
            'api_key': open('api_key').read()
        }
        videos = list(
            VideosProvider(config).get_videos(channel_id))

        self.assertNotEqual(0, len(videos))


if __name__ == '__main__':
    unittest.main()
