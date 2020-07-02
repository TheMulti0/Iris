from collections import namedtuple
from datetime import datetime
from typing import Optional

from pymongo import MongoClient
from pymongo.collection import Collection


UserLatestUpdateTime = namedtuple(
    'UserLatestUpdateTime',
    'user_id latest_update_time')

class UserLatestUpdateTimeRepository:
    __update_times: Collection

    def __init__(self):
        client = MongoClient('localhost', 27017)
        self.__update_times = client['twitterproducerdb']['userlatestupdatetimes']

    def get_user_latest_update_time(self, user_id):
        update_time: Optional[UserLatestUpdateTime] = self.__update_times.find_one(
            {'user_id': user_id})

        if update_time is None:
            return self.insert_new_update_time(
                UserLatestUpdateTime(user_id, datetime.min))

        return update_time

    def set_user_latest_update_time(self, user_id, latest_update_time):
        update_time = self.__update_times.find_one_and_update(
            filter={'user_id': user_id},
            update={'$set': {'latest_update_time': latest_update_time}})

        if update_time is None:
            self.insert_new_update_time(
                UserLatestUpdateTime(user_id, latest_update_time))

    def insert_new_update_time(self, new_update_time):
        return self.__update_times.insert_one(new_update_time)