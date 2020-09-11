from updatesproducer.updateapi.imedia import IMedia


class Audio(IMedia):
    duration_seconds: int
    title: int
    artist: int

    def __init__(
            self,
            url,
            duration_seconds,
            width,
            height):
        self.url = url
        self.duration_seconds = duration_seconds
        self.width = width
        self.height = height

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
