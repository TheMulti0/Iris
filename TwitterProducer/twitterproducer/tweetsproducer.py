import json

from datetime import datetime
from time import sleep

from kafka import KafkaProducer
from twitter_scraper import get_tweets

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
            bootstrap_servers=config.bootstrap_servers,)

    def start(self):
        while True:
            self.update()
            sleep(self.__config.update_interval_seconds)

    def update(self):
        tweets = [
            Tweet(tweet)
            for tweet in get_tweets('@realDonaldTrump', pages=1)
        ]
        updates = [
            UpdateFactory.to_update(tweet)
            for tweet in tweets
        ]
        for update in updates:
            self.send(update)

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
