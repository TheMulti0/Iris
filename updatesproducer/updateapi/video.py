from updatesproducer.updateapi.imedia import IMedia


class Video(IMedia):
    duration_seconds: int
    width: int
    height: int

    def __init__(self, **kwargs):
        self._type = 'Video'
        self.__dict__.update(kwargs)
