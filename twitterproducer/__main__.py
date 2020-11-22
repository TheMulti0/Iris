from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_producer import UpdatesProducer


def create_poller(get_config, repository, cancellation_token):
    def _get_config():
        return get_config()['videos_producer']

    config = _get_config()

    tweets_provider = TweetsProvider(config)

    updates_provider = TwitterUpdatesProvider(tweets_provider)

    producer = UpdatesProducer(
        config,
        VideoDownloader())

    return UpdatesPoller(
        _get_config(),
        producer,
        repository,
        updates_provider,
        cancellation_token)


if __name__ == '__main__':
    Startup('Twitter', create_poller).start()
