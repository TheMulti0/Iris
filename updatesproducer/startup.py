import asyncio
import json
import logging
from threading import Lock, Thread

from kafka import KafkaConsumer

from updatesproducer.cancellation_token import CancellationToken


class Startup:
    def __init__(self, service_name, create_producer):
        self.__service_name = service_name
        self.__create_producer = create_producer

        self.__config = {}
        self.__cancellation_token = CancellationToken()
        self.__config_lock = Lock()

        logging.basicConfig(
            format='[%(asctime)s] [%(name)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.INFO)

    def start(self):

        with self.__config_lock:
            self.__config = json.load(open('appsettings.json'))

        self.run_async(
            self.aggregate(
                self.produce(),
                self.consume_configs()
            )
        )

    @staticmethod
    def run_async(future):
        loop = asyncio.get_event_loop()
        loop.run_until_complete(future)

    @staticmethod
    async def aggregate(*coroutines):
        await asyncio.gather(*coroutines)

    async def produce(self):
        while True:
            with self.__config_lock:
                producer = self.__create_producer(self.__config, self.__cancellation_token)

            await producer.start()

    async def consume_configs(self):
        await asyncio.sleep(1)

        with self.__config_lock:
            consumer = self.create_consumer(self.__config)

        for record in consumer:
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
