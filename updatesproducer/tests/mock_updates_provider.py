from datetime import datetime

from updatesproducer.kafka.iupdates_provider import IUpdatesProvider
from updatesproducer.updateapi.update import Update


class MockUpdatesProvider(IUpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            Update(
                content='Mock updateapi',
                author_id='Mock author',
                creation_date=datetime.now(),
                url='mockurl://updateapi.com',
                media=[],
                repost=False)
        ]