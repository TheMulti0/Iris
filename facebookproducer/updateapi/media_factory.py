from facebookproducer.posts.post import Post

from updatesproducer.updateapi.photo import Photo
from updatesproducer.updateapi.video import Video


class MediaFactory:
    @staticmethod
    def to_media(post: Post):
        return MediaFactory.get_photos(post) + MediaFactory.get_videos(post)

    @staticmethod
    def get_photos(post):
        try:
            if post.image is not None:
                return [
                    Photo(post.image)
                ]
            return []
        except AttributeError:
            return []

    @staticmethod
    def get_videos(post):
        try:
            if post.video is not None:
                return [
                    Video(post.video)
                ]
            return []
        except AttributeError:
            return []
