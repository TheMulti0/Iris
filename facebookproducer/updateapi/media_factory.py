from facebookproducer.posts.post import Post
from updatesproducer.updateapi.media import Media
from updatesproducer.updateapi.mediatype import MediaType


class MediaFactory:
    @staticmethod
    def to_media(post: Post):
        return MediaFactory.get_photos(post) + MediaFactory.get_videos(post)

    @staticmethod
    def get_photos(post):
        try:
            if post.image is not None:
                return [
                    Media(post.image, MediaType.Photo)
                ]
            return []
        except AttributeError:
            return []

    @staticmethod
    def get_videos(post):
        try:
            if post.video is not None:
                return [
                    Media(post.video, MediaType.Video)
                ]
            return []
        except AttributeError:
            return []
