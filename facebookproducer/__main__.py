import json
import logging

from facebookproducer.postsproducer import PostsProducer
from producer.mongodb_config import MongoDbConfig
from producer.updates_repository import UpdatesRepository

from producer.topic_producer_config import TopicProducerConfig


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

    posts_producer = PostsProducer(
        TopicProducerConfig(appsettings['posts_producer']),
        repository,
        logging.getLogger(PostsProducer.__name__))

    posts_producer.start()


if __name__ == '__main__':
    main()
