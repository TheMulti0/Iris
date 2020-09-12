from abc import ABC


class IMedia(ABC):
    _type: str
    url: str
