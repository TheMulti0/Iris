from datetime import datetime

from updatesproducer.db.iupdates_repository import IUpdatesRepository
from updatesproducer.db.user_latest_update_time import UserLatestUpdateTime


class MockUpdatesRepository(IUpdatesRepository):
    def get_user_latest_update_time(self, user_id):
        return UserLatestUpdateTime(
            user_id,
            datetime.min
        ).__dict__

    def set_user_latest_update_time(self, user_id, latest_update_time):
        pass
