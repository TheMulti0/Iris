import json

from twitterproducer.tweetsproducer import TweetsProducer
from twitterproducer.topicproducerconfig import TopicProducerConfig

appsettings = json.load(open('appsettings.json'))

tweets_producer = TweetsProducer(
    TopicProducerConfig(appsettings['tweets_producer']))

tweets_producer.start()
