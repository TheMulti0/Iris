from updatesproducer.updateapi.media import Media
from updatesproducer.updateapi.mediatype import MediaType
from twitterproducer.tweets.tweet import Tweet


class MediaFactory:
    @staticmethod
    def to_media(tweet: Tweet):
        return MediaFactory.get_photos(tweet) + MediaFactory.get_videos(tweet)

    @staticmethod
    def get_photos(tweet):
        try:
            return [
                Media(photo_url, MediaType.Photo)
                for photo_url in tweet.photos
            ]
        except AttributeError:
            return []

    @staticmethod
    def get_videos(tweet):
        try:
            return [
                Media(tweet.video, MediaType.Video)
            ]
        except AttributeError:
            return []
