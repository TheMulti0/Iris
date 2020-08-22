from facebook_scraper import get_posts

from facebookproducer.posts.iposts_provider import IPostsProvider
from facebookproducer.posts.post import Post


class PostsProvider(IPostsProvider):
    def get_posts(self, user_id):
        return [
            Post(post, user_id)
            for post in get_posts(
                user_id,
                pages=1)
        ]
