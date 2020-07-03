import json
import logging

from facebookproducer.postsproducer import PostsProducer
from producer.mongodbconfig import MongoDbConfig
from producer.userlatestupdatetimerepository import UserLatestUpdateTimeRepository

from producer.topicproducerconfig import TopicProducerConfig

logging.basicConfig(
    format='[%(asctime)s] %(message)s',
    datefmt='%Y-%m-%d %H:%M:%S %z',
    level=logging.DEBUG)

appsettings = json.load(open('appsettings.json'))

repository = UserLatestUpdateTimeRepository(
    MongoDbConfig(appsettings['mongodb']),
    logging.getLogger('UserLatestUpdateTimeRepository')
)

posts_producer = PostsProducer(
    TopicProducerConfig(appsettings['posts_producer']),
    repository,
    logging.getLogger('UserLatestUpdateTimeRepository'))

posts_producer.start()
