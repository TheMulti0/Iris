import json
import logging

from producer.mongodbconfig import MongoDbConfig
from producer.producer import Producer
from producer.userlatestupdatetimerepository import UserLatestUpdateTimeRepository

from producer.topicproducerconfig import TopicProducerConfig
from twitterproducer.tweetsprovider import TweetsProvider


def main():
    logging.basicConfig(
        format='[%(asctime)s] %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S %z',
        level=logging.DEBUG)

    appsettings = json.load(open('appsettings.json'))

    repository = UserLatestUpdateTimeRepository(
        MongoDbConfig(appsettings['mongodb']),
        logging.getLogger('UserLatestUpdateTimeRepository')
    )

    tweets_producer = Producer(
        TopicProducerConfig(appsettings['tweets_producer']),
        repository,
        TweetsProvider(),
        logging.getLogger('UserLatestUpdateTimeRepository'))

    tweets_producer.start()


if __name__ == '__main__':
    main()
