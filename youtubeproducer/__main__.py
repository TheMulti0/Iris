import json
import logging

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.kafka.producer import Producer
from updatesproducer.db.updates_repository import UpdatesRepository

from updatesproducer.kafka.topic_producer_config import TopicProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider
from youtubeproducer.videos.videos_provider import VideosProvider


def create_producer(config, cancellation_token):
    repository = UpdatesRepository(
        MongoDbConfig(config['mongodb']),
        logging.getLogger(UpdatesRepository.__name__)
    )

    return Producer(
        TopicProducerConfig(config['videos_producer']),
        repository,
        YouTubeUpdatesProvider(
            VideosProvider(config['videos_producer'])
        ),
        VideoDownloader(logging.getLogger(VideoDownloader.__name__)),
        cancellation_token,
        logging.getLogger(Producer.__name__))


if __name__ == '__main__':
    Startup('YouTube', create_producer).start()
