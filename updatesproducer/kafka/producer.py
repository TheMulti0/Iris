import json

from datetime import datetime
from logging import Logger
from time import sleep

from kafka import KafkaProducer

from updatesproducer.cancellation_token import CancellationToken
from updatesproducer.kafka.iupdates_provider import IUpdatesProvider
from updatesproducer.db.iupdates_repository import IUpdatesRepository

from updatesproducer.kafka.topic_producer_config import TopicProducerConfig
from updatesproducer.updateapi.media import Media
from updatesproducer.updateapi.mediatype import MediaType


class Producer:
    def __init__(
            self,
            config: TopicProducerConfig,
            repository: IUpdatesRepository,
            updates_provider: IUpdatesProvider,
            cancellation_token: CancellationToken,
            logger: Logger):
        self.__config = config
        self.__producer = KafkaProducer(bootstrap_servers=config.bootstrap_servers)
        self.__repository = repository
        self.__updates_provider = updates_provider
        self.__cancellation_token = cancellation_token
        self.__logger = logger

    def start(self):
        while True:
            self.__logger.info('Updating all users')
            self.update()

            if self.__cancellation_token.cancelled:
                return

            interval_seconds = self.__config.update_interval_seconds
            self.__logger.info('Done updating. Sleeping for %s seconds', interval_seconds)
            sleep(interval_seconds)

    def update(self):
        for user in self.__config.watched_users:
            self._update_user(user)

    def _update_user(self, user_id):
        self.__logger.info('Updating user %s', user_id)

        new_updates = self._get_new_updates(user_id)
        self.__logger.debug('Got new updates')

        for update in new_updates:
            self._send(update)
            self.__repository.set_user_latest_update_time(
                user_id,
                update.creation_date)

    def _get_new_updates(self, user_id):
        updates = self.__updates_provider.get_updates(user_id)

        sorted_updates = sorted(
            updates,
            key=lambda u: u.creation_date)

        user_latest_update_time = self.__repository.get_user_latest_update_time(user_id)

        return filter(
            lambda u: u.creation_date > user_latest_update_time['latest_update_time'],
            sorted_updates
        )

    @staticmethod
    def _json_converter(obj):
        if isinstance(obj, datetime):
            return obj.__str__()
        if isinstance(obj, Media):
            return obj.__dict__
        if isinstance(obj, MediaType):
            return obj.value

    def _send(self, update):
        self.__logger.info('Sending update %s to Kafka as JSON UTF-8 bytes', update.url)

        json_str = json.dumps(update.__dict__, default=self._json_converter)
        update_bytes = bytes(json_str, 'utf-8')

        self.__producer.send(
            self.__config.topic,
            value=update_bytes,
            timestamp_ms=int(datetime.now().timestamp()))
