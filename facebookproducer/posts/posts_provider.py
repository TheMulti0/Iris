from abc import ABC

from facebook_scraper import get_posts

from facebookproducer.posts.post import Post


class PostsProvider(ABC):
    def get_posts(self, user_id):
        return [
            Post(post, user_id)
            for post in get_posts(
                user_id,
                pages=1)
        ]
