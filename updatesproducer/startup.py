import asyncio
import json
import logging
import os

import sentry_sdk
from sentry_sdk.integrations.logging import LoggingIntegration
from threading import Lock

from aiokafka import AIOKafkaConsumer

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.db.updates_repository import UpdatesRepository
from updatesproducer.tests.mock_updates_repository import MockUpdatesRepository
from updatesproducer.updateapi.video_downloader import VideoDownloader
from updatesproducer.updates_poller import UpdatesPoller
from updatesproducer.updates_producer import UpdatesProducer


class Startup:
    def __init__(self, service_name, create_updates_provider):
        self.__service_name = service_name
        self.__create_updates_provider = create_updates_provider

        logging.basicConfig(
            format='[%(asctime)s] [%(name)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.INFO)
        self.__logger = logging.getLogger(Startup.__name__)

        self.__config = {}
        self.__config_lock = Lock()

        self.__sentry_logging = LoggingIntegration(
            level=logging.INFO,  # Capture info and above as breadcrumbs
            event_level=logging.ERROR  # Send errors as events
        )

    def start(self):
        config_file_name = self.get_config_file_name()
        self.update_config(json.load(open(config_file_name, encoding='utf-8')))

        config = self.get_config()
        if config.get('sentry'):
            self.__logger.info('Initializing sentry')

            sentry_sdk.init(
                dsn=config['sentry']['dsn'],
                integrations=[self.__sentry_logging]
            )

        self.run_async(
            self.aggregate(
                self.start_poller(),
                self.consume_configs()
            )
        )

    @staticmethod
    def get_config_file_name():
        environment = os.environ.get('ENVIRONMENT')
        if environment:
            return f'appsettings.{environment}.json'
        else:
            return 'appsettings.json'

    @staticmethod
    def run_async(future):
        loop = asyncio.get_event_loop()
        loop.run_until_complete(future)

    @staticmethod
    async def aggregate(*coroutines):
        await asyncio.gather(*coroutines)

    async def start_poller(self):
        config = self.get_config()
        poller = self.create_poller(config)

        self.__logger.info('Starting updates poller')
        await poller.start()

    def create_poller(self, config):
        repository = self.__create_repository(config)

        self.__logger.info('Creating updates poller')

        producer = UpdatesProducer(config['kafka'])
        video_downloader = VideoDownloader(config.get('video_downloader'))
        updates_provider = self.__create_updates_provider(config)

        return UpdatesPoller(
            lambda: self.get_config()['poller'],
            producer,
            repository,
            updates_provider,
            video_downloader)

    def __create_repository(self, config):
        mongo_config = config.get('mongodb')

        if mongo_config is None:
            self.__logger.info('Creating mock repository')
            return MockUpdatesRepository()

        self.__logger.info('Creating mongodb repository')
        return UpdatesRepository(MongoDbConfig(mongo_config))

    async def consume_configs(self):
        await asyncio.sleep(1)

        config = self.get_config()
        self.__logger.info('Creating kafka consumer')
        consumer = self.create_consumer(config)

        try:
            await consumer.start()

            async for message in consumer:
                await self.on_message(message)
        finally:
            await consumer.stop()

    async def on_message(self, message):
        self.__logger.info('Received config {}', message)

        if message.key != bytes(self.__service_name):
            pass

        self.update_config(json.loads(message.value))

    def get_config(self):
        with self.__config_lock:
            return dict(self.__config)

    def update_config(self, new):
        with self.__config_lock:
            self.__config.update(new)

    @staticmethod
    def create_consumer(config):
        kafka_config = config['kafka']

        return AIOKafkaConsumer(
            kafka_config['configs']['topic'],
            bootstrap_servers=kafka_config['bootstrap_servers']
        )
