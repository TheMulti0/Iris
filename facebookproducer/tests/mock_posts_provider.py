from datetime import datetime

from facebookproducer.posts.iposts_provider import IPostsProvider
from facebookproducer.posts.post import Post


class MockPostsProvider(IPostsProvider):
    def get_posts(self, user_id):
        return [
            Post({
                'post_url': '/mock_user/1111',
                'time': datetime.now(),
                'text': 'Mock tweet',
                'image': ['sample-photo-url'],
                'video': ['sample-video-url']
            }, user_id)]