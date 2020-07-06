import json

from datetime import datetime
from logging import Logger
from time import sleep

from kafka import KafkaProducer

from facebookproducer.facebookupdatesprovider import FacebookUpdatesProvider
from producer.updates_repository import UpdatesRepository

from producer.topic_producer_config import TopicProducerConfig


class PostsProducer:

    def __init__(
            self,
            config: TopicProducerConfig,
            repository: UpdatesRepository,
            logger: Logger):

        self.__config = config
        self.__producer = KafkaProducer(
            bootstrap_servers=config.bootstrap_servers)
        self.__repository = repository
        self.__provider = FacebookUpdatesProvider(repository)
        self.__logger = logger

    def start(self):
        while True:
            self.__logger.info('Updating all users')
            self.update()

            interval_seconds = self.__config.update_interval_seconds
            self.__logger.info('Done updating. Sleeping for %s seconds', interval_seconds)
            sleep(interval_seconds)

    def update(self):
        self.update_user('Netanyahu')

    def update_user(self, user_id):
        self.__logger.info('Updating user %s', user_id)

        updates = self.__provider.get_updates(user_id)

        self.__logger.debug('Got new updates')

        for update in updates:
            self.send(update)
            self.__repository.set_user_latest_update_time(user_id, update.creation_date)

    @staticmethod
    def datetime_converter(dt: datetime):
        if isinstance(dt, datetime):
            return dt.__str__()

    def send(self, update):
        self.__logger.info('Sending update %s to Kafka as JSON UTF-8 bytes', update.url)

        dumps = json.dumps(update.__dict__, default=self.datetime_converter)
        update_bytes = bytes(dumps, 'utf-8')

        self.__producer.send(
            self.__config.topic,
            update_bytes)
