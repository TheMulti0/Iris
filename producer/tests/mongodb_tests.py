import json
import logging
import unittest
from datetime import datetime
from unittest import TestCase

from producer.mongodb_config import MongoDbConfig
from producer.updates_repository import UpdatesRepository


class MongoDbTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(MongoDbTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

        appsettings = json.load(open('appsettings.json'))
        self.__mongodb_config = MongoDbConfig(appsettings['mongodb'])

    def test_updates_repository(self):
        repository = UpdatesRepository(
            self.__mongodb_config,
            logging.getLogger(UpdatesRepository.__name__)
        )

        user_id = 'test_user'
        # Mongodb is less accurate with date microseconds
        update_time = datetime.now().replace(microsecond=0)

        repository.set_user_latest_update_time(user_id, update_time)

        # Db returns a dictionary with objectid
        self.assertDictContainsSubset(
            dict(user_id=user_id, latest_update_time=update_time),
            repository.get_user_latest_update_time(user_id)
        )


if __name__ == '__main__':
    unittest.main()
