import logging

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.db.updates_repository import UpdatesRepository

from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_poller_config import UpdatesPollerConfig
from updatesproducer.updates_producer import UpdatesProducer
from updatesproducer.updates_producer_config import UpdatesProducerConfig


def create_pipe(config, cancellation_token):
    repository = UpdatesRepository(
        MongoDbConfig(config['mongodb']),
        logging.getLogger(UpdatesRepository.__name__)
    )

    tweets_provider = TweetsProvider(config['tweets_producer'], logging.getLogger(TweetsProvider.__name__))

    updates_provider = TwitterUpdatesProvider(tweets_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config['tweets_producer']),
        VideoDownloader(logging.getLogger(VideoDownloader.__name__)),
        logging.getLogger(UpdatesProducer.__name__))

    return UpdatesPoller(
        UpdatesPollerConfig(config['tweets_producer']),
        producer,
        repository,
        updates_provider,
        cancellation_token,
        logging.getLogger(UpdatesPoller.__name__))


if __name__ == '__main__':
    Startup('Twitter', create_pipe).start()
