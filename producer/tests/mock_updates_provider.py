from datetime import datetime

from producer.kafka.iupdates_provider import IUpdatesProvider
from producer.updateapi import Update


class MockUpdatesProvider(IUpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            Update(
                content='Mock updateapi',
                creation_date=datetime.now(),
                url='mockurl://updateapi.com')
        ]