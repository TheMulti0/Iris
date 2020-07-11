import json
import logging

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.kafka.producer import Producer
from updatesproducer.db.updates_repository import UpdatesRepository

from updatesproducer.kafka.topic_producer_config import TopicProducerConfig
from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider


def main():
    logging.basicConfig(
        format='[%(asctime)s] %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S %z',
        level=logging.DEBUG)

    appsettings = json.load(open('appsettings.json'))

    repository = UpdatesRepository(
        MongoDbConfig(appsettings['mongodb']),
        logging.getLogger(UpdatesRepository.__name__)
    )

    tweets_provider = TweetsProvider(
        logging.getLogger(TwitterUpdatesProvider.__name__))

    twitter_updates_provider = TwitterUpdatesProvider(tweets_provider)

    tweets_producer = Producer(
        TopicProducerConfig(appsettings['tweets_producer']),
        repository,
        twitter_updates_provider,
        logging.getLogger(Producer.__name__))

    tweets_producer.start()


if __name__ == '__main__':
    main()
