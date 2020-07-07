from datetime import datetime

twitter_base_url = 'https://twitter.com'


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
    hashtags: list
    urls: list
    photos: list
    video: str

    def __init__(self, original_dict):
        self_dict = self.__dict__

        self_dict.update(original_dict)

        self.tweetUrl = f'{twitter_base_url}{self.tweetUrl}'
        self.hashtags = self_dict['entries']['hashtags']
        self.urls = self_dict['entries']['urls']
        self.photos = self_dict['entries']['photos']

        videos = self_dict['entries']['videos']
        if (len(videos) > 0):
            self.video = videos
