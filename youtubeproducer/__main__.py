
from updatesproducer.updates_poller import UpdatesPoller

from updatesproducer.updates_producer_config import UpdatesProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider
from youtubeproducer.videos.videos_provider import VideosProvider


def create_poller(get_config, repository, cancellation_token):
    config = get_config()
    node_name = 'videos_producer'

    videos_provider = VideosProvider(config[node_name])

    updates_provider = YouTubeUpdatesProvider(videos_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config[node_name]),
        VideoDownloader())

    return UpdatesPoller(
        lambda: get_config([node_name]),
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('YouTube', create_poller).start()
