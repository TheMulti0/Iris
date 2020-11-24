from updatesproducer.iupdatesproducer import IUpdatesProducer


class MockUpdatesProducer(IUpdatesProducer):
    async def start(self):
        return
