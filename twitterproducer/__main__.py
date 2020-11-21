from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_producer import UpdatesProducer
from updatesproducer.updates_producer_config import UpdatesProducerConfig


def create_poller(get_config, repository, cancellation_token):
    config = get_config()
    node_name = 'tweets_producer'

    tweets_provider = TweetsProvider(config[node_name])

    updates_provider = TwitterUpdatesProvider(tweets_provider)

    producer = UpdatesProducer(
        UpdatesProducerConfig(config[node_name]),
        VideoDownloader())

    return UpdatesPoller(
        lambda: get_config()[node_name],
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('Twitter', create_poller).start()
