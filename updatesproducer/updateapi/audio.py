from updatesproducer.updateapi.imedia import IMedia


class Audio(IMedia):
    duration_seconds: int
    title: int
    artist: int

    def __init__(self, **kwargs):
        self._type = 'Audio'
        self.__dict__.update(kwargs)
