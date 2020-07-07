from facebookproducer.posts.post import Post
from facebookproducer.updateapi.media_factory import MediaFactory
from producer.updateapi.update import Update


class UpdateFactory:
    @staticmethod
    def to_update(post: Post):
        return Update(
            content=post.text,
            author_id=post.author_id,
            creation_date=post.time,
            url=post.post_url,
            media=MediaFactory.to_media(post)
        )
