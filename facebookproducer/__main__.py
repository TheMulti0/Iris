import json
import logging

from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider
from producer.db.mongodb_config import MongoDbConfig
from producer.kafka.producer import Producer
from producer.db.updates_repository import UpdatesRepository

from producer.kafka.topic_producer_config import TopicProducerConfig


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

    posts_producer = Producer(
        TopicProducerConfig(appsettings['posts_producer']),
        repository,
        FacebookUpdatesProvider(),
        logging.getLogger(Producer.__name__))

    posts_producer.start()


if __name__ == '__main__':
    main()
