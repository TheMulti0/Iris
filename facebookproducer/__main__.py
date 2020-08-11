import json
import logging

from facebookproducer.posts.posts_provider import PostsProvider
from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider
from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.kafka.producer import Producer
from updatesproducer.db.updates_repository import UpdatesRepository

from updatesproducer.kafka.topic_producer_config import TopicProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader


def create_producer(config, cancellation_token):
    repository = UpdatesRepository(
        MongoDbConfig(config['mongodb']),
        logging.getLogger(UpdatesRepository.__name__)
    )

    return Producer(
        TopicProducerConfig(config['posts_producer']),
        repository,
        FacebookUpdatesProvider(
            PostsProvider()
        ),
        VideoDownloader(logging.getLogger(VideoDownloader.__name__)),
        cancellation_token,
        logging.getLogger(Producer.__name__))


if __name__ == '__main__':
    Startup('Facebook', create_producer).start()
