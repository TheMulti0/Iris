from datetime import datetime

from twitterproducer.tweets.itweets_provider import ITweetsProvider
from twitterproducer.tweets.tweet import Tweet


class MockTweetsProvider(ITweetsProvider):
    def get_tweets(self, user_id):
        return [
            Tweet({
                'username': user_id,
                'tweetUrl': '/mock_user/1111',
                'time': datetime.now(),
                'text': 'Mock tweet',
                'entries': {
                    'hashtags': ['#mock'],
                    'urls': [],
                    'photos': ['sample-photo-url'],
                    'videos': ['sample-video-url']
                }
            })]