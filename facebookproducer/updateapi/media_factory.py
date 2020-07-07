from facebookproducer.posts.post import Post
from producer.updateapi.media import Media
from producer.updateapi.mediatype import MediaType
from twitterproducer.tweets.tweet import Tweet


class MediaFactory:
    @staticmethod
    def to_media(post: Post):
        return MediaFactory.get_photos(post) + MediaFactory.get_videos(post)

    @staticmethod
    def get_photos(post):
        try:
            return [
                Media(post.image, MediaType.Photo)
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
