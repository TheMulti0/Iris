from datetime import datetime
from logging import Logger
from typing import Optional

from pymongo import MongoClient
from pymongo.collection import Collection

from producer.db.iupdates_repository import IUpdatesRepository
from producer.db.mongodb_config import MongoDbConfig
from producer.db.user_latest_update_time import UserLatestUpdateTime


class UpdatesRepository(IUpdatesRepository):
    __update_times: Collection

    def __init__(
            self,
            config: MongoDbConfig,
            logger: Logger):

        client = MongoClient(config.connection_string)
        self.__update_times = client[config.db]['userlatestupdatetimes']

        self.__logger = logger
        self.__logger.info('Connected to MongoDB')

    def get_user_latest_update_time(self, user_id):
        update_time: Optional[UserLatestUpdateTime] = self.__update_times.find_one(
            {'user_id': user_id})

        if update_time is None:
            return self._insert_new_update_time(
                UserLatestUpdateTime(user_id, datetime.min).__dict__)

        return update_time

    def set_user_latest_update_time(self, user_id, latest_update_time):
        self.__logger.info('Updating %s latest update time to %s', user_id, latest_update_time)
        update_time = self.__update_times.find_one_and_update(
            filter={'user_id': user_id},
            update={'$set': {'latest_update_time': latest_update_time}})

        if update_time is None:
            self._insert_new_update_time(
                UserLatestUpdateTime(user_id, latest_update_time).__dict__)

    def _insert_new_update_time(self, new_update_time):
        self.__update_times.insert_one(new_update_time)
        return new_update_time
