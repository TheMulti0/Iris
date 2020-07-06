from datetime import datetime

from facebook_scraper import get_posts

from producer.update import Update
from producer.updates_repository import UpdatesRepository


class Post:
    post_id: str
    text: str
    time: datetime
    image: str
    likes: int
    comments: int
    reactions: dict
    post_url: str
    link: str

    def __init__(self, original_dict):
        self.__dict__ = original_dict


class UpdateFactory:
    @staticmethod
    def to_update(post: Post):
        return Update(
            content=post.text,
            creation_date=post.time,
            url=post.post_url
        )


class FacebookUpdatesProvider:

    def __init__(self, repository: UpdatesRepository):
        self.__repository = repository

    def get_updates(self, user_id: str):
        return self.__get_new_updates(
            self.__get_posts(user_id),
            user_id
        )

    @staticmethod
    def __get_posts(user_id):
        return [
            Post(post)
            for post in get_posts(user_id, pages=1)
        ]

    def __get_new_updates(self, posts, user_id):
        updates = [
            UpdateFactory.to_update(post)
            for post in posts
        ]
        sorted_updates = sorted(
            updates,
            key=lambda u: u.creation_date)

        user_latest_update_time = self.__repository.get_user_latest_update_time(user_id)

        return filter(
            lambda u: u.creation_date > user_latest_update_time['latest_update_time'],
            sorted_updates
        )
