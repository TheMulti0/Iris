import logging

from facebookproducer.posts.posts_provider import PostsProvider
from facebookproducer.updateapi.facebook_updates_provider import FacebookUpdatesProvider
from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.db.updates_repository import UpdatesRepository
from updatesproducer.updates_poller_config import UpdatesPollerConfig

from updatesproducer.updates_producer_config import UpdatesProducerConfig
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_producer import UpdatesProducer


def create_pipe(config, cancellation_token):
    repository = UpdatesRepository(
        MongoDbConfig(config['mongodb']),
        logging.getLogger(UpdatesRepository.__name__)
    )

    posts_provider = PostsProvider()

    updates_provider = FacebookUpdatesProvider(posts_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config['posts_producer']),
        cancellation_token,
        logging.getLogger(UpdatesProducer.__name__))

    return UpdatesPoller(
        UpdatesPollerConfig(config['posts_producer']),
        producer,
        repository,
        updates_provider,
        cancellation_token,
        logging.getLogger(UpdatesPoller.__name__))


if __name__ == '__main__':
    Startup('Facebook', create_pipe).start()
