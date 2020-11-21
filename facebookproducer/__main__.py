import logging

from facebookproducer.posts.posts_provider import PostsProvider
from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_poller_config import UpdatesPollerConfig

from updatesproducer.updates_producer_config import UpdatesProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer


def create_poller(config, repository, cancellation_token):
    posts_provider = PostsProvider()

    updates_provider = FacebookUpdatesProvider(posts_provider)

    posts_producer_config = config['posts_producer']

    producer = UpdatesProducer(
        UpdatesProducerConfig(posts_producer_config),
        VideoDownloader({
            'username': posts_producer_config['username'],
            'password': posts_producer_config['password']
        }))

    return UpdatesPoller(
        UpdatesPollerConfig(posts_producer_config),
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('Facebook', create_poller).start()
