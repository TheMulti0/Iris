import unittest
from unittest import TestCase

from facebookproducer.posts_provider import PostsProvider


class PostsProviderTests(TestCase):
    def test1(self):
        user_id = 'Netanyahu'

        posts = PostsProvider().get_updates(user_id)
        self.assertFalse(len(posts) == 0)


if __name__ == '__main__':
    unittest.main()
