from abc import ABC


class IUpdatesProducer(ABC):
    async def start(self):
        pass

    async def send(self, update):
        pass
