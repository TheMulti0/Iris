import logging

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.db.updates_repository import UpdatesRepository
from updatesproducer.updates_poller_config import UpdatesPollerConfig

from updatesproducer.updates_producer_config import UpdatesProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider
from youtubeproducer.videos.videos_provider import VideosProvider


def create_pipe(config, cancellation_token):
    repository = UpdatesRepository(
        MongoDbConfig(config['mongodb']),
        logging.getLogger(UpdatesRepository.__name__)
    )

    videos_provider = VideosProvider(config['videos_producer'])

    updates_provider = YouTubeUpdatesProvider(videos_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config['updates_producer']),
        VideoDownloader(logging.getLogger(VideoDownloader.__name__)),
        logging.getLogger(UpdatesProducer.__name__))

    return UpdatesPoller(
        UpdatesPollerConfig(config['updates_poller']),
        producer,
        repository,
        updates_provider,
        cancellation_token,
        logging.getLogger(UpdatesPoller.__name__))


if __name__ == '__main__':
    Startup('YouTube', create_pipe).start()
