from datetime import datetime

from producer.iupdates_provider import IUpdatesProvider
from producer.update import Update


class MockUpdatesProvider(IUpdatesProvider):
    def get_updates(self, user_id: str):
        return [
            Update(
                content='Mock update',
                creation_date=datetime.now(),
                url='mockurl://update.com')
        ]