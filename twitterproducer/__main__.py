import json
import logging
from threading import Lock, Thread

from kafka import KafkaConsumer

from updatesproducer.db.mongodb_config import MongoDbConfig
from updatesproducer.kafka.cancellation_token import CancellationToken
from updatesproducer.kafka.producer import Producer
from updatesproducer.db.updates_repository import UpdatesRepository

from updatesproducer.kafka.topic_producer_config import TopicProducerConfig
from twitterproducer.tweets.tweets_provider import TweetsProvider
from twitterproducer.updateapi.twitter_updates_provider import TwitterUpdatesProvider
from updatesproducer.tests.mock_updates_repository import MockUpdatesRepository


class Startup:
    def __init__(self):
        self.__config = {}
        self.__cancellation_token = CancellationToken()
        self.__config_lock = Lock()

    def main(self):
        logging.basicConfig(
            format='[%(asctime)s] %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S %z',
            level=logging.INFO)

        with self.__config_lock:
            self.__config = json.load(open('appsettings.json'))

        Thread(target=self.operate).start()

        with self.__config_lock:
            consumer = self.create_consumer(self.__config)

        for record in consumer:
            if record.key != b'Twitter':
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

    def operate(self):
        while True:
            with self.__config_lock:
                producer = self.create_producer(self.__config)

            producer.start()

    def create_producer(self, config):
        repository = UpdatesRepository(
            MongoDbConfig(config['mongodb']),
            logging.getLogger(UpdatesRepository.__name__))

        tweets_provider = TweetsProvider(
            logging.getLogger(TwitterUpdatesProvider.__name__))

        twitter_updates_provider = TwitterUpdatesProvider(tweets_provider)

        return Producer(
            TopicProducerConfig(config['tweets_producer']),
            MockUpdatesRepository(),
            twitter_updates_provider,
            self.__cancellation_token,
            logging.getLogger(Producer.__name__))


if __name__ == '__main__':
    Startup().main()
