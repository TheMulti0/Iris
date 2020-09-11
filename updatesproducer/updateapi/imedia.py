from abc import ABC


class IMedia(ABC):
    url: str
    thumbnail_url: str
