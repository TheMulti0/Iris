from producer.update.mediatype import MediaType


class Media:
    url: str
    type: MediaType

    def __init__(
            self,
            url: str,
            type: MediaType):
        self.url = url
        self.type = type