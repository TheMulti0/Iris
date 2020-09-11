from updatesproducer.updateapi.imedia import IMedia


class Video(IMedia):
    duration_seconds: int
    width: int
    height: int

    def __init__(self, url):
        self.url = url

    def __init__(
            self,
            url,
            thumbnail_url,
            duration_seconds,
            width,
            height):
        self.url = url
        self.thumbnail_url = thumbnail_url
        self.duration_seconds = duration_seconds
        self.width = width
        self.height = height
