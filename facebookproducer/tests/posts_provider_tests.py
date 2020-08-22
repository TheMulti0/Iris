import logging
import unittest
from unittest import TestCase

from facebookproducer.posts.posts_provider import PostsProvider


class PostsProviderTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(PostsProviderTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

    def test1(self):
        user_id = 'Netanyahu'

        posts = list(
            PostsProvider().get_posts(user_id))

        self.assertNotEqual(0, len(posts))


if __name__ == '__main__':
    unittest.main()
