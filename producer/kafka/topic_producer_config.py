class TopicProducerConfig:
    topic: str
    bootstrap_servers: str
    update_interval_seconds: float

    def __init__(self, original_dict):
        self.__dict__.update(original_dict)
