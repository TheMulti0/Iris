import json

from datetime import datetime
from logging import Logger
from time import sleep

from kafka import KafkaProducer
from facebook_scraper import get_posts

from producer.update import Update
from producer.userlatestupdatetimerepository import UserLatestUpdateTimeRepository

from producer.topicproducerconfig import TopicProducerConfig


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


class PostsProducer:
    def __init__(
            self,
            config: TopicProducerConfig,
            repository: UserLatestUpdateTimeRepository,
            logger: Logger):
        self.__config = config

        self.__producer = KafkaProducer(
            bootstrap_servers=config.bootstrap_servers)

        self.__repository = repository

        self.__logger = logger

    def start(self):
        while True:
            self.__logger.info('Updating all users')
            self.update()

            interval_seconds = self.__config.update_interval_seconds
            self.__logger.info('Done updating. Sleeping for %s seconds', interval_seconds)
            sleep(interval_seconds)

    def update(self):
        self.update_user('Netanyahu')

    def update_user(self, user_id):
        self.__logger.info('Updating user %s', user_id)
        posts = self.get_posts(user_id)

        new_updates = self.get_new_updates(posts, user_id)
        self.__logger.debug('Got new updates')

        for update in new_updates:
            self.send(update)
            self.__repository.set_user_latest_update_time(user_id, update.creation_date)

    def get_posts(self, user_id):
        return [
            Post(post)
            for post in get_posts(user_id, pages=1)
        ]

    def get_new_updates(self, posts, user_id):
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

    @staticmethod
    def datetime_converter(dt: datetime):
        if isinstance(dt, datetime):
            return dt.__str__()

    def send(self, update):
        self.__logger.info('Sending update %s to Kafka as JSON UTF-8 bytes', update.url)

        dumps = json.dumps(update.__dict__, default=self.datetime_converter)
        update_bytes = bytes(dumps, 'utf-8')

        self.__producer.send(
            self.__config.topic,
            update_bytes)
