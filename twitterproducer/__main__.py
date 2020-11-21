import logging

from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_poller_config import UpdatesPollerConfig
from updatesproducer.updates_producer import UpdatesProducer
from updatesproducer.updates_producer_config import UpdatesProducerConfig


def create_poller(config, repository, cancellation_token):
    tweets_provider = TweetsProvider(config['tweets_producer'])

    updates_provider = TwitterUpdatesProvider(tweets_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config['tweets_producer']),
        VideoDownloader())

    return UpdatesPoller(
        UpdatesPollerConfig(config['tweets_producer']),
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('Twitter', create_poller).start()
