import logging

from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_poller_config import UpdatesPollerConfig

from updatesproducer.updates_producer_config import UpdatesProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider
from youtubeproducer.videos.videos_provider import VideosProvider


def create_poller(config, repository, cancellation_token):
    videos_provider = VideosProvider(config['videos_producer'])

    updates_provider = YouTubeUpdatesProvider(videos_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config['videos_producer']),
        VideoDownloader(logging.getLogger(VideoDownloader.__name__)),
        logging.getLogger(UpdatesProducer.__name__))

    return UpdatesPoller(
        UpdatesPollerConfig(config['videos_producer']),
        producer,
        repository,
        updates_provider,
        cancellation_token,
        logging.getLogger(UpdatesPoller.__name__))


if __name__ == '__main__':
    Startup('YouTube', create_poller).start()
