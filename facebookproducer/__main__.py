from facebookproducer.posts.posts_provider import PostsProvider
from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider
from updatesproducer.updates_poller import UpdatesPoller

from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer


def create_updates_provider(config):
    posts_provider = PostsProvider()

    return FacebookUpdatesProvider(posts_provider)


if __name__ == '__main__':
    Startup('Facebook', create_updates_provider).start()
