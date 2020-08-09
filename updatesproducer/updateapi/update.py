from datetime import datetime
from typing import List

from updatesproducer.updateapi.media import Media


class Update:
    content: str
    author_id: str
    creation_date: datetime
    url: str
    media: List[Media]
    repost: bool

    def __init__(
            self,
            content: str,
            author_id: str,
            creation_date: datetime,
            url: str,
            media: list,
            repost: bool):
        self.content = content
        self.author_id = author_id
        self.creation_date = creation_date
        self.url = url
        self.media = media
        self.repost = repost
