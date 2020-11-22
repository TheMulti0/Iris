from updatesproducer.updates_poller import UpdatesPoller

from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider
from youtubeproducer.videos.videos_provider import VideosProvider


def create_poller(get_config, repository, cancellation_token):
    def _get_config():
        return get_config()['videos_producer']

    config = _get_config()

    videos_provider = VideosProvider(config)

    updates_provider = YouTubeUpdatesProvider(videos_provider)

    producer = UpdatesProducer(
        config,
        VideoDownloader())

    return UpdatesPoller(
        _get_config,
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('YouTube', create_poller).start()
