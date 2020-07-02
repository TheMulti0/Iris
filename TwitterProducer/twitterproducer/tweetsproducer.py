import json

from datetime import datetime
from time import sleep

from kafka import KafkaProducer
from twitter_scraper import get_tweets

from twitterproducer.databaserepository import UserLatestUpdateTime, UserLatestUpdateTimeRepository
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
    def __init__(self, config: TopicProducerConfig):
        self.__config = config

        self.__producer = KafkaProducer(
            bootstrap_servers=config.bootstrap_servers)

        self.__repository = UserLatestUpdateTimeRepository()

    def start(self):
        while True:
            self.update()
            sleep(self.__config.update_interval_seconds)

    def update(self):
        self.update_user('@realDonaldTrump')

    def update_user(self, user_id):
        tweets = self.get_tweets(user_id)

        new_updates = self.get_new_updates(tweets, user_id)

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
        sorted_updates = list(sorted(
            updates,
            key=lambda u: u.creation_date))

        user_latest_update_time = self.__repository.get_user_latest_update_time(user_id)

        return list(filter(
            lambda u: u.creation_date > user_latest_update_time,
            sorted_updates
        ))

    @staticmethod
    def datetime_converter(dt: datetime):
        if isinstance(dt, datetime):
            return dt.__str__()

    def send(self, update):
        dumps = json.dumps(update.__dict__, default=self.datetime_converter)
        update_bytes = bytes(dumps, 'utf-8')

        self.__producer.send(
            self.__config.topic,
            update_bytes)
