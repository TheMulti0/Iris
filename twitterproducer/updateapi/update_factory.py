from producer.updateapi.update import Update
from twitterproducer.updateapi.media_factory import MediaFactory
from twitterproducer.tweets.tweet import Tweet


class UpdateFactory:
    @staticmethod
    def to_update(tweet: Tweet):
        return Update(
            content=tweet.text,
            author_id=tweet.username,
            creation_date=tweet.time,
            url=tweet.tweetUrl,
            media=MediaFactory.to_media(tweet)
        )
