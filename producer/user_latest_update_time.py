from datetime import datetime


class UserLatestUpdateTime:
    user_id: str
    latest_update_time: datetime

    def __init__(
            self,
            user_id: str,
            latest_update_time: datetime):
        self.user_id = user_id
        self.latest_update_time = latest_update_time

    def __init__(self, d):
        self.__dict__ = d
