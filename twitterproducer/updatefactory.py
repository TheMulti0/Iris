from producer.update import Update
from twitterproducer.tweet import Tweet


class UpdateFactory:
    @staticmethod
    def to_update(tweet: Tweet):
        return Update(
            content=tweet.text,
            creation_date=tweet.time,
            url=tweet.tweetUrl
        )
