import asyncio
import json
import logging
import sentry_sdk
from sentry_sdk.integrations.logging import LoggingIntegration
from threading import Lock

from kafka import KafkaConsumer

from updatesproducer.cancellation_token import CancellationToken
from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.db.updates_repository import UpdatesRepository
from updatesproducer.tests.mock_updates_repository import MockUpdatesRepository


class Startup:
    def __init__(self, service_name, create_pipe):
        self.__service_name = service_name
        self.__create_poller = create_pipe

        logging.basicConfig(
            format='[%(asctime)s] [%(name)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.INFO)
        self.__logger = logging.getLogger(Startup.__name__)

        self.__config = {}
        self.__cancellation_token = CancellationToken()
        self.__config_lock = Lock()

        self.__sentry_logging = LoggingIntegration(
            level=logging.INFO,  # Capture info and above as breadcrumbs
            event_level=logging.ERROR  # Send errors as events
        )

    def start(self):
        with self.__config_lock:
            self.__config = json.load(open('appsettings.json', encoding='utf-8'))
            
            if self.__config.get('sentry'):
                self.__logger.info('Initializing sentry')

                sentry_sdk.init(
                    dsn=self.__config['sentry']['dsn'],
                    integrations=[self.__sentry_logging]
                )

        self.run_async(
            self.aggregate(
                self.start_poller(),
                # self.consume_configs()
            )
        )

    @staticmethod
    def run_async(future):
        loop = asyncio.get_event_loop()
        loop.run_until_complete(future)

    @staticmethod
    async def aggregate(*coroutines):
        await asyncio.gather(*coroutines)

    async def start_poller(self):
        with self.__config_lock:
            repository = self.__create_repository()

            self.__logger.info('Creating updates poller')
            poller = self.__create_poller(self.__config, repository, self.__cancellation_token)

        self.__logger.info('Starting updates poller')
        await poller.start()

    def __create_repository(self):
        config = self.__config.get('mongodb')

        if config is None:
            self.__logger.info('Creating mock repository')
            return MockUpdatesRepository()

        self.__logger.info('Creating mongodb repository')
        return UpdatesRepository(
            MongoDbConfig(config),
            logging.getLogger(UpdatesRepository.__name__)
        )

    async def consume_configs(self):
        await asyncio.sleep(1)

        with self.__config_lock:
            self.__logger.info('Creating kafka consumer')
            consumer = self.create_consumer(self.__config)

        for record in consumer:
            self.__logger.info('Received config')

            if record.key != bytes(self.__service_name):
                continue

            with self.__config_lock:
                self.__config.update(json.loads(record.value))

            self.__cancellation_token.cancel()
            self.__cancellation_token = CancellationToken()

    @staticmethod
    def create_consumer(config):
        config_consumer_config = config['config_consumer']

        return KafkaConsumer(
            config_consumer_config['topic'],
            bootstrap_servers=config_consumer_config['bootstrap_servers']
        )
