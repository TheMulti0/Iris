from facebookproducer.post import Post
from producer.update import Update


class UpdateFactory:
    @staticmethod
    def to_update(post: Post):
        return Update(
            content=post.text,
            creation_date=post.time,
            url=post.post_url
        )
