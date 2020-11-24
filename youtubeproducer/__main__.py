from updatesproducer.updates_poller import UpdatesPoller

from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer
from youtubeproducer.updateapi.youtube_updates_provider import YouTubeUpdatesProvider
from youtubeproducer.videos.videos_provider import VideosProvider


def create_updates_provider(config):
    videos_provider = VideosProvider(config['youtube'])

    return YouTubeUpdatesProvider(videos_provider)


if __name__ == '__main__':
    Startup('YouTube', create_updates_provider).start()
