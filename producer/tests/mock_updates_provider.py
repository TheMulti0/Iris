from datetime import datetime

from producer.kafka.iupdates_provider import IUpdatesProvider
from producer.updateapi.update import Update


class MockUpdatesProvider(IUpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            Update(
                content='Mock updateapi',
                author_id='Mock author',
                creation_date=datetime.now(),
                url='mockurl://updateapi.com',
                media=[])
        ]