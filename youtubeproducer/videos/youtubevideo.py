class YouTubeVideo:
    video_id: str
    channelId: str
    title: str
    description: str
    thumbnails: dict
    publishedAt: str
    publishTime: str
    channelTitle: str
    liveBroadcastContent: str

    def __init__(self, original_dict, video_id):
        self.__dict__.update(original_dict)
        self.video_id = video_id
