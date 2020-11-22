from facebookproducer.posts.posts_provider import PostsProvider
from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider
from updatesproducer.updates_poller import UpdatesPoller

from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer


def create_poller(get_config, repository, cancellation_token):
    def _get_config():
        return get_config()['posts_producer']

    config = _get_config()

    posts_provider = PostsProvider()

    updates_provider = FacebookUpdatesProvider(posts_provider)

    producer = UpdatesProducer(
        config,
        VideoDownloader({
            'username': config.get('username'),
            'password': config.get('password')
        }))

    return UpdatesPoller(
        _get_config,
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('Facebook', create_poller).start()
