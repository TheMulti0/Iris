from updatesproducer.updateapi.imedia import IMedia


class Photo(IMedia):

    def __init__(self, **kwargs):
        self.type = 'Photo'
        self.__dict__.update(kwargs)
