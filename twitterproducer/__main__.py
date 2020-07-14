import logging

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.kafka.producer import Producer
from updatesproducer.db.updates_repository import UpdatesRepository

from updatesproducer.kafka.topic_producer_config import TopicProducerConfig
from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.startup import Startup


def create_producer(config, cancellation_token):
    repository = UpdatesRepository(
        MongoDbConfig(config['mongodb']),
        logging.getLogger(UpdatesRepository.__name__))

    tweets_provider = TweetsProvider(
        logging.getLogger(TwitterUpdatesProvider.__name__))

    twitter_updates_provider = TwitterUpdatesProvider(tweets_provider)

    return Producer(
        TopicProducerConfig(config['tweets_producer']),
        repository,
        twitter_updates_provider,
        cancellation_token,
        logging.getLogger(Producer.__name__))


if __name__ == '__main__':
    Startup('Twitter', create_producer).start()
