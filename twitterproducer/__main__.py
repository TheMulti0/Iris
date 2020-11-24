from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.startup import Startup
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_producer import UpdatesProducer


def create_updates_provider(config):
    tweets_provider = TweetsProvider(config['twitter'])

    return TwitterUpdatesProvider(tweets_provider)


if __name__ == '__main__':
    Startup('Twitter', create_updates_provider).start()
