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
    should_redownload_video: bool

    def __init__(
            self,
            content: str,
            author_id: str,
            creation_date: datetime,
            url: str,
            media: list,
            repost: bool,
            should_redownload_video: bool):
        self.content = content
        self.author_id = author_id
        self.creation_date = creation_date
        self.url = url
        self.media = media
        self.repost = repost
        self.should_redownload_video = should_redownload_video
