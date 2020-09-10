from updatesproducer.updateapi.mediatype import MediaType


class Media:
    url: str
    type: MediaType
    thumbnail_url: str
    duration_seconds: int
    width: int
    height: int

    def __init__(
            self,
            url: str,
            type: MediaType):
        self.url = url
        self.type = type

    def __init__(
            self,
            url: str,
            type: MediaType,
            thumbnail_url: str,
            duration_seconds: int,
            width: int,
            height: int):
        self.url = url
        self.type = type
        self.thumbnail_url = thumbnail_url
        self.duration_seconds = duration_seconds
        self.width = width
        self.height = height
