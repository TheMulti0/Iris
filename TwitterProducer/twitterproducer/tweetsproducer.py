import json

from datetime import datetime
from logging import Logger
from time import sleep

from kafka import KafkaProducer
from twitter_scraper import get_tweets

from twitterproducer.userlatestupdatetimerepository import UserLatestUpdateTime, UserLatestUpdateTimeRepository
from twitterproducer.topicproducerconfig import TopicProducerConfig
from twitterproducer.update import Update


class Tweet:
    tweetId: str
    userId: str
    username: str
    tweetUrl: str
    isRetweet: bool
    isPinned: bool
    time: datetime
    text: str
    replies: int
    retweets: int
    likes: int
    entries: dict

    def __init__(self, original_dict):
        self.__dict__ = original_dict


class UpdateFactory:
    @staticmethod
    def to_update(tweet: Tweet):
        return Update(
            content=tweet.text,
            creation_date=tweet.time,
            url=tweet.tweetUrl
        )


class TweetsProducer:
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
        self.update_user('@realDonaldTrump')

    def update_user(self, user_id):
        self.__logger.info('Updating user %s', user_id)
        tweets = self.get_tweets(user_id)

        new_updates = self.get_new_updates(tweets, user_id)
        self.__logger.debug('Got new updates')

        for update in new_updates:
            self.send(update)
            self.__repository.set_user_latest_update_time(user_id, update.creation_date)

    def get_tweets(self, user_id):
        tweets = [
            Tweet(tweet)
            for tweet in get_tweets(user_id, pages=1)
        ]
        return tweets

    def get_new_updates(self, tweets, user_id):
        updates = [
            UpdateFactory.to_update(tweet)
            for tweet in tweets
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
