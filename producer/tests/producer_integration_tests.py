import json
import logging
import unittest
from unittest import TestCase

from kafka import KafkaConsumer

from producer.producer import Producer
from producer.tests.mock_updates_provider import MockUpdatesProvider
from producer.tests.mock_updates_repository import MockUpdatesRepository
from producer.topic_producer_config import TopicProducerConfig


class ProducerIntegrationTests(TestCase):
    def __init__(self, *args, **kwargs):
        super(ProducerIntegrationTests, self).__init__(*args, **kwargs)

        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.DEBUG)

        appsettings = json.load(open('appsettings.json'))
        self.__topic_producer_config = TopicProducerConfig(appsettings['tests_producer'])

    def test1(self):
        producer = Producer(
            self.__topic_producer_config,
            MockUpdatesRepository(),
            MockUpdatesProvider(),
            logging.getLogger('UserLatestUpdateTimeRepository')
        )

        producer.update()

        consumer = KafkaConsumer(
            'updates',
            bootstrap_servers=self.__topic_producer_config.bootstrap_servers,
            auto_offset_reset='earliest')

        self.assertIsNotNone(next(consumer))  # assert first message is not None


if __name__ == '__main__':
    unittest.main()
