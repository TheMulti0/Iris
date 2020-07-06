from datetime import datetime


class Tweet:
    tweetId: str
    userId: str
    username: str
    tweetUrl: str
    isRetweet: bool
    isPinned: bool
    time: datetime
    text: str
    replies: int
    retweets: int
    likes: int
    entries: dict

    def __init__(self, original_dict):
        self.__dict__ = original_dict
