from updatesproducer.updateapi.media import Media
from updatesproducer.updateapi.mediatype import MediaType
from twitterproducer.tweets.tweet import Tweet


class MediaFactory:
    @staticmethod
    def to_media(tweet: Tweet):
        # Videos are downloaded later, if any
        return [
            Media(photo_url, MediaType.Photo)
            for photo_url in tweet.photos
        ]
