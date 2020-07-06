from datetime import datetime


class Post:
    post_id: str
    text: str
    time: datetime
    image: str
    likes: int
    comments: int
    reactions: dict
    post_url: str
    link: str

    def __init__(self, original_dict):
        self.__dict__.update(original_dict)