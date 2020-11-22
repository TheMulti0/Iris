from abc import ABC


class IUpdatesProducer(ABC):
    def send(self, update):
        pass
