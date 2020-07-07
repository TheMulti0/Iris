from facebookproducer.posts.iposts_provider import IPostsProvider
from facebookproducer.updateapi.update_factory import UpdateFactory
from producer.kafka.iupdates_provider import IUpdatesProvider


class FacebookUpdatesProvider(IUpdatesProvider):
    __posts_provider: IPostsProvider

    def __init__(self, posts_provider):
        self.__posts_provider = posts_provider

    def get_updates(self, user_id: str):
        return [
            UpdateFactory.to_update(post)
            for post in self.__posts_provider.get_posts(user_id)
        ]
