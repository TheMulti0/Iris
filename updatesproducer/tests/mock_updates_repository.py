import logging
from datetime import datetime

from updatesproducer.db.iupdates_repository import IUpdatesRepository
from updatesproducer.db.user_latest_update_time import UserLatestUpdateTime


class MockUpdatesRepository(IUpdatesRepository):
    def __init__(self):
        self.__logger = logging.getLogger(MockUpdatesRepository.__name__)

    def get_user_latest_update_time(self, user_id):
        return UserLatestUpdateTime(
            user_id,
            datetime.min
        ).__dict__

    def set_user_latest_update_time(self, user_id, latest_update_time):
        self.__logger.info('')
        pass

    def was_update_sent(self, url):
        self.__logger.info(f'Update with url {url} was sent call, replying False')
        return False

    def update_sent(self, url):
        self.__logger.info(f'Update with url {url} sent')
        pass
