from abc import ABC


class ITweetsProvider(ABC):
    def get_tweets(self, user_id):
        pass