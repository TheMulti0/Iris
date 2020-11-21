import json
import logging
import unittest
from datetime import datetime
from unittest import TestCase

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.db.updates_repository import UpdatesRepository


class MongoDbTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(MongoDbTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

        appsettings = json.load(open('appsettings.json'))
        self.__mongodb_config = MongoDbConfig(appsettings['mongodb'])
        self.__repository = UpdatesRepository(self.__mongodb_config)

    def test_update_times(self):
        user_id = 'test_user'
        # Mongodb is less accurate with date microseconds
        update_time = datetime.now().replace(microsecond=0)

        self.__repository.set_user_latest_update_time(user_id, update_time)

        # Db returns a dictionary with objectid
        self.assertDictContainsSubset(
            dict(user_id=user_id, latest_update_time=update_time),
            self.__repository.get_user_latest_update_time(user_id)
        )

    def test_update_sent(self):
        url = 'url'

        self.__repository.update_sent(url)

        print(self.__repository.was_update_sent(url))


if __name__ == '__main__':
    unittest.main()
