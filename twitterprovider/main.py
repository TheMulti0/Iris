from kafka import KafkaProducer
import json
import time


class Update:
    def __init__(self, content):
        self.Content = content


producer = KafkaProducer(bootstrap_servers='localhost:9092')
i = 0
while True:
    i += 1
    dict__ = Update(f'Message from Python #{i}').__dict__
    msg = json.dumps(dict__)

    producer.send(
        'test-topic',
        bytes(msg, 'utf-8'))

    time.sleep(5)
