from datetime import datetime


class Update:
    content: str
    creation_date: datetime
    url: str

    def __init__(
            self,
            content: str,
            creation_date: datetime,
            url: str):
        self.content = content
        self.creation_date = creation_date
        self.url = url
