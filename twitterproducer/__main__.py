import json
import logging

from producer.mongodb_config import MongoDbConfig
from producer.producer import Producer
from producer.updates_repository import UpdatesRepository

from producer.topic_producer_config import TopicProducerConfig
from twitterproducer.tweets_provider import TweetsProvider


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

    tweets_producer = Producer(
        TopicProducerConfig(appsettings['tweets_producer']),
        repository,
        TweetsProvider(),
        logging.getLogger(Producer.__name__))

    tweets_producer.start()


if __name__ == '__main__':
    main()
