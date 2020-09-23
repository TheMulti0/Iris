import json
import logging
import unittest
from unittest import TestCase

from kafka import KafkaConsumer

from updatesproducer.cancellation_token import CancellationToken
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.tests.mock_updates_provider import MockUpdatesProvider
from updatesproducer.tests.mock_updates_repository import MockUpdatesRepository
from updatesproducer.updates_producer_config import UpdatesProducerConfig


class ProducerIntegrationTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(ProducerIntegrationTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

        appsettings = json.load(open('appsettings.json'))
        self.__topic_producer_config = UpdatesProducerConfig(appsettings['tests_producer'])

    def test1(self):
        producer = UpdatesPoller(
            self.__topic_producer_config,
            MockUpdatesRepository(),
            MockUpdatesProvider(),
            CancellationToken(),
            logging.getLogger(UpdatesPoller.__name__)
        )

        producer.poll()

        consumer = KafkaConsumer(
            'updates',
            bootstrap_servers=self.__topic_producer_config.bootstrap_servers,
            auto_offset_reset='earliest')

        first_message = next(consumer)
        print(f'first_message: {first_message}')
        self.assertIsNotNone(first_message)  # assert first message is not None


if __name__ == '__main__':
    unittest.main()
