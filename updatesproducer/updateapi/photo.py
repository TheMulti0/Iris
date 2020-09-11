from updatesproducer.updateapi.imedia import IMedia


class Photo(IMedia):
    def __init__(self, url):
        self.url = url

    def __init__(self, url, thumbnail_url):
        self.url = url
        self.thumbnail_url = thumbnail_url