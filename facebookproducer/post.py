from datetime import datetime


class Post:
    post_id: str
    text: str
    post_text: str
    shared_text: str
    time: datetime
    image: str
    video: str
    likes: int
    comments: int
    shares: int
    post_url: str
    link: str

    def __init__(self, original_dict):
        self.__dict__.update(original_dict)
