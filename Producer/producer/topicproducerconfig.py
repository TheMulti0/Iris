class TopicProducerConfig:
    topic: str
    bootstrap_servers: str
    update_interval_seconds: float

    def __init__(self, json_dict):
        self.__dict__ = json_dict
