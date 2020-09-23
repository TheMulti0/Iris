class UpdatesProducerConfig:
    topic: str
    key: str
    bootstrap_servers: str

    def __init__(self, original_dict):
        self.__dict__.update(original_dict)
