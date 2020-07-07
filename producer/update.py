from datetime import datetime


class Update:
    content: str
    author_id: str
    creation_date: datetime
    url: str
    media: list

    def __init__(
            self,
            content: str,
            author_id: str,
            creation_date: datetime,
            url: str,
            media: list):
        self.content = content
        self.author_id = author_id
        self.creation_date = creation_date
        self.url = url
        self.media = media
