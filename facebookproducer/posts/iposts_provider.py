from abc import ABC


class IPostsProvider(ABC):
    def get_posts(self, user_id):
        pass
