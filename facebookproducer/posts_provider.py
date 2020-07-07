from facebook_scraper import get_posts

from facebookproducer.update_factory import UpdateFactory
from facebookproducer.post import Post
from producer.iupdates_provider import IUpdatesProvider


class PostsProvider(IUpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            UpdateFactory.to_update(post)
            for post in self._get_posts(user_id)
        ]

    @staticmethod
    def _get_posts(user_id):
        return [
            Post(post)
            for post in get_posts(user_id, pages=1, youtube_dl=True)
        ]
